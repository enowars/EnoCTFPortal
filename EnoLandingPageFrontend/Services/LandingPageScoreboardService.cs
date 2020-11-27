using EnoCore;
using EnoCore.Scoreboard;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class LandingPageScoreboardService
    {
        public Scoreboard? LatestScoreboard { get; set; }
        public delegate void ScoreboardEventHandler(object sender, Scoreboard sb);
        public event ScoreboardEventHandler? ScoreboardEvent;

        private readonly ILogger<LandingPageScoreboardService> logger;
        private readonly HttpClient httpClient;

        public LandingPageScoreboardService(ILogger<LandingPageScoreboardService> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var _ = PollTask();
        }

        public async Task PollTask()
        {
            while (true)
            {
                try
                {
                    var scoreboard = await this.httpClient.GetFromJsonAsync<Scoreboard>("/scoreboard/scoreboard.json", EnoCoreUtil.CamelCaseEnumConverterOptions);
                    if (scoreboard != null && LatestScoreboard?.CurrentRound != scoreboard.CurrentRound)
                    {
                        LatestScoreboard = scoreboard;
                        ScoreboardEvent?.Invoke(this, scoreboard);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"{e.ToFancyString()}");
                }
                await Task.Delay(5000);
            }
        }
    }
}
