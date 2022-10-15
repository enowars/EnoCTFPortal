using EnoCore;
using EnoCore.Models.Scoreboard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class LandingPageScoreboardApiService
    {
        public delegate void NewScoreboardEventHandler(Scoreboard sb);
        public delegate void OldScoreboardEventHandler(Scoreboard oldScoreboard);
        public event NewScoreboardEventHandler? NewScoreboardEvent;

        private readonly ILogger<LandingPageScoreboardApiService> logger;
        private readonly ConcurrentDictionary<long, Scoreboard?> Scoreboards = new();
        private readonly HttpClient httpClient;

        public LandingPageScoreboardApiService(ILogger<LandingPageScoreboardApiService> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var _ = PollTask();
        }

        public Scoreboard? LatestScoreboard { get; set; }

        public async Task<Scoreboard?> TryGetOrRequest(long roundId, bool ignoreFails = false)
        {
            if (this.Scoreboards.TryGetValue(roundId, out var sb))
            {
                return sb;
            }
            else
            {
                return await RequestScoreboard(roundId, ignoreFails);
            }
        }

        private async Task<Scoreboard?> RequestScoreboard(long roundId, bool ignoreFails)
        {
            try
            {
                var scoreboard = await this.httpClient.GetFromJsonAsync<Scoreboard>($"/scoreboard/scoreboard{roundId}.json", EnoCoreUtil.CamelCaseEnumConverterOptions);
                if (scoreboard != null)
                {
                    Scoreboards[roundId] = scoreboard;
                    return scoreboard;
                }
            }
            catch (Exception e)
            {
                if (!ignoreFails)
                {
                    logger.LogError($"{e.ToFancyString()}");
                }
            }
            return null;
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
                        Scoreboards[scoreboard.CurrentRound] = scoreboard;
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
