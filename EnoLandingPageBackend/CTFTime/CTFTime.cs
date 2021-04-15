namespace EnoLandingPageBackend.CTFTime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class CTFTime
    {
        private static HttpClient client = new HttpClient();

        public static async Task<CTFTimeTeamInfo?> GetTeamInfo(long ctftimeId, CancellationToken token)
        {
            var info = await client.GetFromJsonAsync<CTFTimeTeamInfo>($"https://ctftime.org/api/v1/teams/{ctftimeId}/", token);
            return info;
        }
    }
}
