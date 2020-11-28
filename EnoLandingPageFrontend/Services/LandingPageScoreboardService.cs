using EnoCore;
using EnoCore.Scoreboard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class LandingPageScoreboardService
    {
        public delegate void NewScoreboardEventHandler(Scoreboard sb);
        public delegate void OldScoreboardEventHandler(Scoreboard oldScoreboard);
        public event NewScoreboardEventHandler? NewScoreboardEvent;
        public event OldScoreboardEventHandler? OldScoreboardEvent;

        private readonly ILogger<LandingPageScoreboardService> logger;
        private readonly Dictionary<long, Scoreboard?> Scoreboards = new();
        private readonly HttpClient httpClient;

        public LandingPageScoreboardService(ILogger<LandingPageScoreboardService> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var _ = PollTask();
        }

        public Scoreboard? LatestScoreboard { get; set; }

        public bool TryGetOrRequest(long roundId, out Scoreboard? sb, bool ignoreFails = false)
        {
            if (!this.Scoreboards.TryGetValue(roundId, out sb))
            {
                var _ = RequestScoreboard(roundId, ignoreFails);
                return false;
            }
            return true;
        }

        private async Task RequestScoreboard(long roundId, bool ignoreFails)
        {
            try
            {
                var scoreboard = await this.httpClient.GetFromJsonAsync<Scoreboard>($"/scoreboard/scoreboard{roundId}.json", EnoCoreUtil.CamelCaseEnumConverterOptions);
                if (scoreboard != null)
                {
                    Scoreboards[roundId] = scoreboard;
                    OldScoreboardEvent?.Invoke(scoreboard);
                }
            }
            catch (Exception e)
            {
                if (!ignoreFails)
                {
                    logger.LogError($"{e.ToFancyString()}");
                }
            }
        }

        private async Task PollTask()
        {
            while (true)
            {
                try
                {
                    var scoreboard = await this.httpClient.GetFromJsonAsync<Scoreboard>("/scoreboard/scoreboard.json", EnoCoreUtil.CamelCaseEnumConverterOptions);
                    if (scoreboard != null && LatestScoreboard?.CurrentRound != scoreboard.CurrentRound)
                    {
                        if (scoreboard.CurrentRound > 1)
                        {
                            TryGetOrRequest(scoreboard.CurrentRound.Value - 1, out var _);
                        }

                        Scoreboards[scoreboard.CurrentRound.Value] = scoreboard;
                        LatestScoreboard = scoreboard;
                        NewScoreboardEvent?.Invoke(scoreboard);
                    }
                    // TODO invalidate scoreboards dict on restart
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
