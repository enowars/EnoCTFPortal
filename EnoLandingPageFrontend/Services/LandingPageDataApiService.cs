using EnoLandingPageCore;
using EnoLandingPageCore.Database;
using EnoLandingPageCore.Hetzner;
using EnoLandingPageCore.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class LandingPageDataApiService
    {
        private readonly ILogger<LandingPageDataApiService> logger;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public LandingPageDataApiService(ILogger<LandingPageDataApiService> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            this.jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<TeamDetailsMessage> GetTeamInfo()
        {
            return JsonSerializer.Deserialize<TeamDetailsMessage>(await this.httpClient.GetStringAsync("/api/account/info"), jsonOptions)!;
        }

        public async Task<TeamsMessage> GetTeams()
        {
            return JsonSerializer.Deserialize<TeamsMessage>(await this.httpClient.GetStringAsync("/api/data/teams"), jsonOptions)!;
        }

        public async Task<CtfInfoMessage> GetCtfInfo()
        {
            return JsonSerializer.Deserialize<CtfInfoMessage>(await this.httpClient.GetStringAsync("/api/data/ctfinfo"), jsonOptions)!;
        }


        public async Task CheckIn()
        {
            await this.httpClient.PostAsync("/api/account/checkin", null!);
        }

        public async Task StartVm()
        {
            HttpResponseMessage response;
            try
            {
                response = await this.httpClient.PostAsync("/api/vulnbox/startvulnbox", null!);
            }
            catch (Exception e)
            {
                throw new LandingPageServiceException("StartVm request failed.", e);
            }
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content == nameof(ServerExistsException))
                {
                    logger.LogError("StartVm throwing ServerNameInUseException");
                    throw new ServerExistsException("");
                }
                else if (content == nameof(OtherRequestRunningException))
                {
                    throw new OtherRequestRunningException();
                }
                else
                {
                    throw new Exception("Unexpected backend message");
                }
            }
            else
            {
                logger.LogInformation($"StartVm succeeded ({response.StatusCode})");
            }
        }

        public async Task ResetVm()
        {
            HttpResponseMessage response;
            try
            {
                response = await this.httpClient.PostAsync("/api/vulnbox/resetvulnbox", null!);
            }
            catch (Exception e)
            {
                throw new LandingPageServiceException("ResetVm request failed.", e);
            }
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content == nameof(ServerNotExistsException))
                {
                    logger.LogError("ResetVm throwing ServerNotExistsException");
                    throw new ServerNotExistsException();
                }
                else if (content == nameof(OtherRequestRunningException))
                {
                    throw new OtherRequestRunningException();
                }
                else
                {
                    throw new Exception("Unexpected backend message");
                }
            }
            else
            {
                logger.LogInformation($"StartVm succeeded ({response.StatusCode})");
            }
        }
    }
}
