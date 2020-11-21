using EnoLandingPageCore;
using EnoLandingPageCore.Database;
using EnoLandingPageCore.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class EnoLandingPageService
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public EnoLandingPageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<TeamDetailsMessage> GetTeamInfo()
        {
            return JsonSerializer.Deserialize<TeamDetailsMessage>(await this.httpClient.GetStringAsync("/api/account/info"), jsonOptions)!;
        }

        public async Task<List<ConfirmedTeamMessage>> GetConfirmed()
        {
            return JsonSerializer.Deserialize<List<ConfirmedTeamMessage>>(await this.httpClient.GetStringAsync("/api/teams/confirmed"), jsonOptions)!;
        }

        public async Task<CtfInfoMessage> GetCtfInfo()
        {
            return JsonSerializer.Deserialize<CtfInfoMessage>(await this.httpClient.GetStringAsync("/api/ctf/info"), jsonOptions)!;
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task CheckIn()
        {
            await this.httpClient.PostAsync("/api/account/checkin", null);
        }

        public async Task StartVm()
        {
            await this.httpClient.PostAsync("/api/vm/startvulnbox", null);
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
}
