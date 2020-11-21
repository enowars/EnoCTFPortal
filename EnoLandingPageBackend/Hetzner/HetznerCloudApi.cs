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

    public sealed class HetznerCloudApi : IHostedService, IDisposable
    {
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
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.httpClient = new HttpClient();
            this.landingPageSettings = landingPageSettings;
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", landingPageSettings.HetznerCloudApiToken);
        }

        /// <summary>
        /// Enqueue a hetzner api call.
        /// </summary>
        /// <param name="teamId">The corresponding landing page team id.</param>
        /// <param name="call">The method that should be invoked.</param>
        /// <param name="token">A cancellation token which may abort the call.</param>
        /// <returns>Task representing the api call.</returns>
        public static Task Call(long teamId, HetznerCloudApiCallType call, CancellationToken token)
        {
            var tcs = new TaskCompletionSource();
            if (Tasks.TryAdd(teamId, new HetznerCloudApiScheduledCall(call, tcs, token)))
            {
                return tcs.Task;
            }
            else
            {
                throw new Exception("There is already a task scheduled or running for this team");
            }
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
            this.cancellationSource.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(
                async () => await this.HetznerWorker(),
                this.cancellationSource.Token,
                TaskCreationOptions.RunContinuationsAsynchronously,
                TaskScheduler.Default);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.cancellationSource.Cancel();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Conduct an hcloud get server api call for a given team. MUST NOT be called concurrently for an individual team, and everything MUST NOT write to the Teams row while it is running.
        /// TODO prevent login from doing exactly that.
        /// </summary>
        /// <param name="teamId">The corresponding landing page team id.</param>
        /// <param name="call">The method that should be invoked.</param>
        /// <param name="token">A cancellation token which may abort the call.</param>
        /// <returns>Task representing the api call.</returns>
        private async Task DoGetServer(long teamId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            try
            {
                var rootPassword = ""; // File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}{teamId}{Path.DirectorySeparatorChar}root.pw");

                // Call "Get Servers" endpoint.
                dynamic createVmRequest = new JObject();
                createVmRequest.name = $"team{teamId}";
                var jsonContent = JsonConvert.SerializeObject(createVmRequest);
                this.logger.LogInformation($"DoGetServer {jsonContent}");

                var content = new StringContent(jsonContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await this.httpClient.GetAsync(new Uri($"https://api.hetzner.cloud/v1/servers?name=team{teamId}"), token);
                var responseString = await response.Content.ReadAsStringAsync(token);
                this.logger.LogDebug(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Invalid Status Code {response.StatusCode}\n{responseString}");
                }

                // Extract the id and ip
                var result = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                long serverId = result.servers[0].id;
                string ipv4 = result.servers[0].public_net.ipv4.ip;

                // Everything went as well!
                // Update the db record.
                await LandingPageDatabase.UpdateTeamVm(
                    this.serviceProvider,
                    teamId,
                    serverId,
                    ipv4,
                    rootPassword,
                    LandingPageVulnboxStatus.Created);

                // Remove the task from scheduling.
                Tasks.TryRemove(teamId, out var _);

                // Complete the tcs.
                call.Tcs.SetResult();
            }
            catch (Exception e)
            {
                // Something did not go as planned. Set the status to "None".
                await LandingPageDatabase.SetVmStatus(this.serviceProvider, teamId, LandingPageVulnboxStatus.None, token);
                this.logger.LogError($"{nameof(this.DoGetServer)} failed: {e}");

                // If the task has not been completed yet, complete it now with the exception
                if (Tasks.TryRemove(teamId, out var _))
                {
                    call.Tcs.TrySetException(e);
                }
            }
        }

        /// <summary>
        /// Conduct an hcloud create server api call for a given team. MUST NOT be called concurrently for an individual team, and everything MUST NOT write to the Teams row while it is running.
        /// TODO prevent login from doing exactly that.
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
                // Set the status to "Creating".
                await LandingPageDatabase.UpdateTeamVm(this.serviceProvider, teamId, null, null, null, LandingPageVulnboxStatus.Creating);

                // Call "Create Server" endpoint.
                var user_data = ""; // File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}{teamId}{Path.DirectorySeparatorChar}user_data.sh");
                var rootPassword = ""; // File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}{teamId}{Path.DirectorySeparatorChar}root.pw");
                dynamic createVmRequest = new JObject();
                createVmRequest.name = $"team{teamId}";
                createVmRequest.server_type = this.landingPageSettings.HetznerVulnboxType;
                createVmRequest.image = this.landingPageSettings.HetznerVulnboxImage;
                createVmRequest.ssh_keys = new JArray(this.landingPageSettings.HetznerVulnboxPubkey);
                createVmRequest.location = this.landingPageSettings.HetznerVulnboxLocation;
                createVmRequest.user_data = user_data;
                var jsonContent = JsonConvert.SerializeObject(createVmRequest);
                this.logger.LogDebug($"DoCreateServer {jsonContent}");

                var content = new StringContent(jsonContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await this.httpClient.PostAsync("https://api.hetzner.cloud/v1/servers", content, token);
                var responseString = await response.Content.ReadAsStringAsync(token);
                this.logger.LogDebug($"StartVm {responseString}");

                if (responseString.Contains("uniqueness_error"))
                {
                    // Hetzner says the name already exists.
                    throw new ServerExistsException();
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Invalid Status Code {response.StatusCode}\n{responseString}");
                }

                this.logger.LogInformation(responseString);
                var result = JsonConvert.DeserializeObject<dynamic>(responseString)!;
                long serverId = result.server.id;
                string ipv4 = result.server.public_net.ipv4.ip;

                // Everything went as well!
                // Update the db record.
                await LandingPageDatabase.UpdateTeamVm(
                    this.serviceProvider,
                    teamId,
                    serverId,
                    ipv4,
                    rootPassword,
                    LandingPageVulnboxStatus.Created);

                // Remove the task from scheduling.
                Tasks.TryRemove(teamId, out var _);

                // Complete the tcs.
                call.Tcs.SetResult();
            }
            catch (ServerExistsException)
            {
                // We should know this server but we don't. Schedule a Get for that server.
                var scheduledGetCall = new HetznerCloudApiScheduledCall(HetznerCloudApiCallType.Get, call.Tcs, token);
                Tasks.AddOrUpdate(teamId, scheduledGetCall, (_, _) => scheduledGetCall);
            }
            catch (Exception e)
            {
                // Something did not go as planned. Set the status to "None".
                await LandingPageDatabase.SetVmStatus(this.serviceProvider, teamId, LandingPageVulnboxStatus.None, token);
                this.logger.LogError($"{nameof(this.DoCreateServer)} failed: {e}");

                // If the task has not been completed yet, complete it now with the exception
                if (Tasks.TryRemove(teamId, out var _))
                {
                    call.Tcs.TrySetException(e);
                }
            }
        }

        private async Task DoResetServer(long teamId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            /*
            try
            {
                // Call "Get Servers" endpoint.
                dynamic createVmRequest = new JObject();
                createVmRequest.name = $"team{teamId}";
                var jsonContent = JsonConvert.SerializeObject(createVmRequest);
                this.logger.LogInformation($"DoGetServer {jsonContent}");

                var content = new StringContent(jsonContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await this.httpClient.GetAsync(new Uri($"https://api.hetzner.cloud/v1/servers/{}/actions/reset"), token);
                var responseString = await response.Content.ReadAsStringAsync(token);
                this.logger.LogDebug(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Invalid Status Code {response.StatusCode}\n{responseString}");
                }
            }
            catch (Exception e)
            {

            }
            */
        }

        private async Task HetznerWorker()
        {
            // TODO: Discuss which tokens we want to use here.
            while (!this.cancellationSource.Token.IsCancellationRequested)
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
                            if (scheduledApiCall.CallType == HetznerCloudApiCallType.Create)
                            {
                                if (await LandingPageDatabase.GetVmStatus(this.serviceProvider, teamId, this.cancellationSource.Token) == LandingPageVulnboxStatus.Created)
                                {
                                    scheduledApiCall.Tcs.SetException(new ServerExistsException());
                                    Tasks.TryRemove(teamId, out var _);
                                }
                                else
                                {
                                    var t = Task.Run(async () => await this.DoCreateServer(teamId, scheduledApiCall, scheduledApiCall.Token), scheduledApiCall.Token);
                                    await Task.Delay(1100, this.cancellationSource.Token);
                                }
                            }
                            else if (scheduledApiCall.CallType == HetznerCloudApiCallType.Reset)
                            {
                                hadWork = true;
                                var t = Task.Run(async () => await this.DoResetServer(teamId, scheduledApiCall, scheduledApiCall.Token), scheduledApiCall.Token);
                                await Task.Delay(1100, this.cancellationSource.Token);
                            }
                            else if (scheduledApiCall.CallType == HetznerCloudApiCallType.Get)
                            {
                                var t = Task.Run(async () => await this.DoGetServer(teamId, scheduledApiCall, scheduledApiCall.Token), scheduledApiCall.Token);
                                await Task.Delay(1100, this.cancellationSource.Token);
                            }
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
