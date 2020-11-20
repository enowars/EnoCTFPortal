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

        public async Task<TeamInfo> GetTeamInfo()
        {
            return JsonSerializer.Deserialize<TeamInfo>(await this.httpClient.GetStringAsync("/api/team/teaminfo"), jsonOptions)!;
        }
    }
}
