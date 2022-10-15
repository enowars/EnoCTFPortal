namespace EnoLandingPageBackend.Hetzner
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Database;
    using EnoLandingPageCore.Hetzner;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public enum HetznerCloudApiCallType
    {
        Create,
        Reset,
        Get,
    }

    public sealed class HetznerCloudApi : IDisposable
    {
        private const int HetznerApiCallDelay = 1100;
        private static readonly ConcurrentDictionary<long, HetznerCloudApiScheduledCall> Tasks = new();
        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly ILogger<HetznerCloudApi> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly HttpClient httpClient;
        private readonly LandingPageSettings landingPageSettings;

        public HetznerCloudApi(
            ILogger<HetznerCloudApi> logger,
            IServiceProvider serviceProvider,
            LandingPageSettings landingPageSettings)
        {
            logger.LogDebug("HetznerCloudApi()");
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.httpClient = new HttpClient();
            this.landingPageSettings = landingPageSettings;
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", landingPageSettings.HetznerCloudApiToken);
            Task.Factory.StartNew(
                async () => await this.HetznerWorker(this.cancellationSource.Token),
                this.cancellationSource.Token,
                TaskCreationOptions.RunContinuationsAsynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Enqueue a hetzner api call.
        /// </summary>
        /// <param name="teamId">The corresponding landing page team id.</param>
        /// <param name="call">The method that should be invoked.</param>
        /// <returns>Task representing the api call.</returns>
        public Task Call(long teamId, HetznerCloudApiCallType call)
        {
            this.logger.LogDebug($"Call({teamId}, {call})");
            var tcs = new TaskCompletionSource();
            if (Tasks.TryAdd(teamId, new HetznerCloudApiScheduledCall(call, tcs)))
            {
                return tcs.Task;
            }
            else
            {
                throw new OtherRequestRunningException();
            }
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
            this.cancellationSource.Dispose();
        }

        /// <summary>
        /// Conduct an hcloud get server api call for a given team. MUST NOT be called concurrently for an individual team, and everything MUST NOT write to the <see cref="LandingPageDatabaseContext.Vulnboxes"/> row while it is running.
        /// </summary>
        /// <param name="teamId">The corresponding landing page team id.</param>
        /// <param name="call">The method that should be invoked.</param>
        /// <param name="token">A cancellation token which may abort the call.</param>
        /// <returns>Task representing the api call.</returns>
        private async Task DoGetServer(long teamId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            try
            {
                // TODO refactor this into always reading the file?
                var rootPassword = File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}teamdata{Path.DirectorySeparatorChar}team{teamId}{Path.DirectorySeparatorChar}root.pw");

                // Call "Get Servers" endpoint.
                this.logger.LogInformation($"{nameof(this.DoGetServer)} for team {teamId}");
                var response = await this.httpClient.GetAsync(new Uri($"https://api.hetzner.cloud/v1/servers?name=team{teamId}"), token);
                var responseString = await response.Content.ReadAsStringAsync(token);
                this.logger.LogDebug($"Received response in {nameof(this.DoGetServer)} for team {teamId}: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HetznerException($"Invalid Status Code for team {teamId}: {response.StatusCode}\n{responseString}");
                }

                // Extract the id and ip
                var result = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                long serverId = result.servers[0].id;
                string ipv4 = result.servers[0].public_net.ipv4.ip;

                // Everything went as well!
                // Update the db record.
                await LandingPageDatabase.UpdateTeamVulnbox(
                    this.serviceProvider,
                    teamId,
                    serverId,
                    ipv4,
                    rootPassword,
                    LandingPageVulnboxStatus.Created,
                    token);

                // Remove the task from scheduling.
                Tasks.TryRemove(teamId, out var _);

                // Complete the tcs.
                call.Tcs.SetResult();
            }
            catch (Exception e)
            {
                // Something did not go as planned. Set the status to "None".
                await LandingPageDatabase.UpdateTeamVulnbox(
                    this.serviceProvider,
                    teamId,
                    null,
                    null,
                    null,
                    LandingPageVulnboxStatus.None,
                    token);
                this.logger.LogError($"{nameof(this.DoGetServer)} failed: {e}");

                // If the task has not been completed yet, complete it now with the exception
                if (Tasks.TryRemove(teamId, out var _))
                {
                    call.Tcs.TrySetException(e);
                }
            }
        }

        /// <summary>
        /// Conduct an hcloud create server api call for a given team. MUST NOT be called concurrently for an individual team, and everything MUST NOT write to the <see cref="LandingPageDatabaseContext.Vulnboxes"/> row while it is running.
        /// If the server already exists, a <see cref="DoGetServer"/> will be scheduled.
        /// </summary>
        /// <param name="teamId">The corresponding landing page team id.</param>
        /// <param name="call">The method that should be invoked.</param>
        /// <param name="token">A cancellation token which may abort the call.</param>
        /// <returns>Task representing the api call.</returns>
        private async Task DoCreateServer(long teamId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            try
            {
                this.logger.LogDebug($"DoCreateServer({teamId}, {call}");

                // Set the status to "Creating".
                await LandingPageDatabase.UpdateTeamVulnbox(
                    this.serviceProvider,
                    teamId,
                    null,
                    null,
                    null,
                    LandingPageVulnboxStatus.Creating,
                    token);

                // Call "Create Server" endpoint.
                var user_data = File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}teamdata{Path.DirectorySeparatorChar}team{teamId}{Path.DirectorySeparatorChar}user_data.sh");
                var rootPassword = File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}teamdata{Path.DirectorySeparatorChar}team{teamId}{Path.DirectorySeparatorChar}root.pw");
                dynamic createVmRequest = new JObject();
                createVmRequest.name = $"team{teamId}";
                createVmRequest.server_type = this.landingPageSettings.HetznerVulnboxType;
                createVmRequest.image = this.landingPageSettings.HetznerVulnboxImage;
                createVmRequest.ssh_keys = new JArray(this.landingPageSettings.HetznerVulnboxPubkey);
                createVmRequest.location = this.landingPageSettings.HetznerVulnboxLocation;
                createVmRequest.user_data = user_data;
                var jsonContent = JsonConvert.SerializeObject(createVmRequest);
                this.logger.LogDebug($"{nameof(this.DoCreateServer)} for team {teamId}: {jsonContent}");

                var content = new StringContent(jsonContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await this.httpClient.PostAsync("https://api.hetzner.cloud/v1/servers", content, token);
                var responseString = await response.Content.ReadAsStringAsync(token);
                this.logger.LogDebug($"StartVm for team {teamId}: {responseString}");

                if (responseString.Contains("uniqueness_error"))
                {
                    // Hetzner says the name already exists.
                    throw new ServerExistsException("responseString contained uniqueness_error");
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new HetznerException($"Invalid Status Code for team {teamId}: {response.StatusCode}\n{responseString}");
                }

                this.logger.LogInformation(responseString);
                var result = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                long serverId = result.server.id;
                string ipv4 = result.server.public_net.ipv4.ip;

                // Everything went as well!
                // Update the db record.
                await LandingPageDatabase.UpdateTeamVulnbox(
                    this.serviceProvider,
                    teamId,
                    serverId,
                    ipv4,
                    rootPassword,
                    LandingPageVulnboxStatus.Created,
                    token);

                // Remove the task from scheduling.
                Tasks.TryRemove(teamId, out var _);

                // Complete the tcs.
                call.Tcs.SetResult();
            }
            catch (ServerExistsException)
            {
                // We should know this server but we don't. Schedule a Get for that server.
                var scheduledGetCall = new HetznerCloudApiScheduledCall(HetznerCloudApiCallType.Get, call.Tcs);
                Tasks.AddOrUpdate(teamId, scheduledGetCall, (_, _) => scheduledGetCall);
            }
            catch (Exception e)
            {
                // Something did not go as planned. Set the status to "None".
                await LandingPageDatabase.UpdateTeamVulnbox(
                    this.serviceProvider,
                    teamId,
                    null,
                    null,
                    null,
                    LandingPageVulnboxStatus.None,
                    token);
                this.logger.LogError($"{nameof(this.DoCreateServer)} for team {teamId} failed: {e}");

                // If the task has not been completed yet, complete it now with the exception
                if (Tasks.TryRemove(teamId, out var _))
                {
                    call.Tcs.TrySetException(e);
                }
            }
        }

        private async Task DoResetServer(long teamId, long hetznerServerId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            try
            {
                // Call "Reset Server" endpoint.
                this.logger.LogInformation($"{nameof(this.DoResetServer)} for team {teamId}");
                var response = await this.httpClient.PostAsync(new Uri($"https://api.hetzner.cloud/v1/servers/{hetznerServerId}/actions/reset"), null!, token);
                var responseString = await response.Content.ReadAsStringAsync(token);
                this.logger.LogDebug(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HetznerException($"Invalid Status Code {response.StatusCode}\n{responseString}");
                }

                // Remove the task from scheduling.
                Tasks.TryRemove(teamId, out var _);

                // Complete the tcs.
                call.Tcs.SetResult();
            }
            catch (Exception e)
            {
                this.logger.LogError($"{nameof(this.DoResetServer)} failed: {e}");

                // If the task has not been completed yet, complete it now with the exception
                if (Tasks.TryRemove(teamId, out var _))
                {
                    call.Tcs.TrySetException(e);
                }
            }
        }

        /// <summary>
        /// The service provided by this IHostedService.
        /// Takes scheduled api calls from the Dictionary and runs them with appropriate limits for the rate limits.
        /// </summary>
        /// <returns>The task representing the service.</returns>
        private async Task HetznerWorker(CancellationToken token)
        {
            // TODO: Discuss which tokens we want to use here.
            while (!token.IsCancellationRequested)
            {
                bool hadWork = false;
                foreach (var teamId in Tasks.Keys.ToArray())
                {
                    if (Tasks.TryGetValue(teamId, out var scheduledApiCall))
                    {
                        if (!scheduledApiCall.IsRunning)
                        {
                            // This task is the only task that reads and writes this value, so we should not need explicit synchronization.
                            scheduledApiCall.IsRunning = true;
                            this.logger.LogDebug($"HetznerWorker scheduling API call {teamId}, {scheduledApiCall}");
                            var vulnbox = await LandingPageDatabase.GetTeamVulnbox(this.serviceProvider, teamId, token);
                            if (scheduledApiCall.CallType == HetznerCloudApiCallType.Create)
                            {
                                if (vulnbox.VulnboxStatus == LandingPageVulnboxStatus.Created)
                                {
                                    scheduledApiCall.Tcs.SetException(new ServerExistsException($"VulnboxStatus (team {teamId}) is LandingPageVulnboxStatus.Created"));
                                    Tasks.TryRemove(teamId, out var _);
                                }
                                else
                                {
                                    var t = Task.Run(async () => await this.DoCreateServer(teamId, scheduledApiCall, token), token);
                                    await Task.Delay(HetznerApiCallDelay, this.cancellationSource.Token);
                                }
                            }
                            else if (scheduledApiCall.CallType == HetznerCloudApiCallType.Reset)
                            {
                                if (vulnbox.HetznerServerId is long hetznerServerId)
                                {
                                    hadWork = true;
                                    var t = Task.Run(async () => await this.DoResetServer(teamId, hetznerServerId, scheduledApiCall, token), token);
                                    await Task.Delay(HetznerApiCallDelay, this.cancellationSource.Token);
                                }
                                else
                                {
                                    this.logger.LogWarning($"Reset for non-existing server (team {teamId}");
                                    scheduledApiCall.Tcs.SetException(new ServerNotExistsException());
                                    Tasks.TryRemove(teamId, out var _);
                                }
                            }
                            else if (scheduledApiCall.CallType == HetznerCloudApiCallType.Get)
                            {
                                var t = Task.Run(async () => await this.DoGetServer(teamId, scheduledApiCall, token), token);
                                await Task.Delay(HetznerApiCallDelay, this.cancellationSource.Token);
                            }

                            this.logger.LogDebug($"HetznerWorker scheduling API call {teamId}, {scheduledApiCall} finished");
                        }
                    }
                }

                if (!hadWork)
                {
                    await Task.Delay(10, this.cancellationSource.Token);
                }
            }
        }
    }
}
