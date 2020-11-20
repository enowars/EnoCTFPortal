using EnoLandingPageCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class EnoLandingPageDataService
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public EnoLandingPageDataService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<LandingPageTeamInfo> GetTeamInfo()
        {
            return JsonSerializer.Deserialize<LandingPageTeamInfo>(await this.httpClient.GetStringAsync("/api/account/info"), jsonOptions)!;
        }

        public async Task<List<LandingPageTeam>> GetConfirmed()
        {
            return JsonSerializer.Deserialize<List<LandingPageTeam>>(await this.httpClient.GetStringAsync("/api/teams/confirmed"), jsonOptions)!;
        }

        public async Task<LandingPageCtfInfo> GetCtfInfo()
        {
            return JsonSerializer.Deserialize<LandingPageCtfInfo>(await this.httpClient.GetStringAsync("/api/ctf/info"), jsonOptions)!;
        }

        public async Task CheckIn()
        {
            await this.httpClient.PostAsync("/api/account/checkin", null);
        }
    }
}
