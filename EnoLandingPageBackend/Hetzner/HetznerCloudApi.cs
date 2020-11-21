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
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public enum HetznerCloudApiCall
    {
        Create,
        Reset,
    }

    public class HetznerCloudApi : IHostedService
    {
        private const string UserDataPath = "./data/user_data.sh";
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
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static Task Call(long teamId, HetznerCloudApiCall call, CancellationToken token)
        {
            var tcs = new TaskCompletionSource();
            if (Tasks.TryAdd(teamId, new HetznerCloudApiScheduledCall(call, token, tcs, false)))
            {
                return tcs.Task;
            }
            else
            {
                throw new Exception("There is already a task scheduled or running for this team");
            }
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

        private async Task DoStartVm(long teamId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            try
            {
                await LandingPageDatabase.UpdateTeamVm(this.serviceProvider, teamId, null, null, LandingPageVulnboxStatus.Starting);
                dynamic createVmRequest = new JObject();
                createVmRequest.name = $"team{teamId}";
                createVmRequest.server_type = this.landingPageSettings.HetznerVulnboxType;
                createVmRequest.image = this.landingPageSettings.HetznerVulnboxImage;
                createVmRequest.ssh_keys = new JArray(this.landingPageSettings.HetznerVulnboxPubkey);
                createVmRequest.location = this.landingPageSettings.HetznerVulnboxLocation;
                createVmRequest.user_data = File.ReadAllText(UserDataPath);

                var jsonContent = JsonConvert.SerializeObject(createVmRequest);
                this.logger.LogInformation($"StartVm {jsonContent}");
                var response = await this.httpClient.PostAsync("https://api.hetzner.cloud/v1/servers", new StringContent(jsonContent), token);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Invalid Status Code {response.StatusCode}");
                }

                var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync(token))!;
                long serverId = result.server.id;
                string ipv4 = result.server.public_net.ipv4.ip;

                await LandingPageDatabase.UpdateTeamVm(this.serviceProvider, teamId, serverId, ipv4, LandingPageVulnboxStatus.Started);
                Tasks.TryRemove(teamId, out var _);
                call.Tcs.SetResult();
            }
            catch (Exception e)
            {
                this.logger.LogError($"{nameof(this.DoStartVm)} failed: {e}");
                if (Tasks.TryRemove(teamId, out var _))
                {
                    call.Tcs.TrySetException(e);
                }
            }
        }

        private Task DoResetVm(long teamId, HetznerCloudApiScheduledCall call, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private async Task HetznerWorker()
        {
            while (!this.cancellationSource.Token.IsCancellationRequested)
            {
                bool hadWork = false;
                foreach (var teamId in Tasks.Keys.ToArray())
                {
                    if (Tasks.TryGetValue(teamId, out var task))
                    {
                        if (!task.IsRunning)
                        {
                            hadWork = true;
                            task.IsRunning = true;
                            if (task.Call == HetznerCloudApiCall.Create)
                            {
                                var t = Task.Run(async () => await this.DoStartVm(teamId, task, task.Token), task.Token);
                            }
                            else
                            {
                                var t = Task.Run(async () => await this.DoResetVm(teamId, task, task.Token), task.Token);
                            }
                        }
                    }

                    await Task.Delay(1100, this.cancellationSource.Token);
                }

                if (!hadWork)
                {
                    await Task.Delay(10, this.cancellationSource.Token);
                }
            }
        }
    }
}
