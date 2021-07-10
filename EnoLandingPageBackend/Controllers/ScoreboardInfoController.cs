namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using EnoCore.Scoreboard;
    using Microsoft.Extensions.Logging;
    using EnoLandingPageCore;
    using Microsoft.Extensions.Caching.Memory;
    using EnoLandingPageBackend.Cache;
    using EnoLandingPageBackend.Database;
    using System.Threading;
    using System.IO;
    using System.Text.Json;
    using EnoLandingPageBackend.Models;

    /// <summary>
    /// Retrieve the scoreboard.
    /// If you change this controller you will also have to change the static hosting variant.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class ScoreboardInfoController : ControllerBase
    {
        private readonly ILogger<ScoreboardInfoController> logger;
        private readonly LandingPageSettings settings;
        private ScoreboardCache _cache;

        /// <summary>
        /// Initalizes the ScoreboardController.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        public ScoreboardInfoController(ILogger<ScoreboardInfoController> logger, LandingPageSettings settings, ScoreboardCache cache)
        {
            this.logger = logger;
            this.settings = settings;
            this._cache = cache;
        }

        /// <summary>
        /// Gets the current scoreboard.
        /// </summary>
        /// <returns>The scoreboard of the current round.</returns>
        [HttpGet]
        [Route("scoreboard.json")]
        public async Task<ActionResult<OverrideScoreboard>> GetDefaultScoreboard(CancellationToken cancellationToken)
        {
            string scoreboard;
            scoreboard = this._cache.TryGetDefault();
            if (scoreboard == null)
            {
                using (var reader = System.IO.File.OpenText(this.getScoreboardFilePath()))
                {
                    scoreboard = await reader.ReadToEndAsync();
                    this.logger.LogInformation(scoreboard);
                    this._cache.CreateDefault(scoreboard);
                }
            }
            if (scoreboard == null)
            {
                return NotFound();
            }
            return Content(scoreboard);
        }

        /// <summary>
        /// Gets the scoreboard of a given roundId.
        /// </summary>
        /// <param name="roundId">Number of the round.</param>
        /// <returns>The scoreboard of the given roundId.</returns>
        [HttpGet]
        [Route("scoreboard{roundId}.json")]
        public async Task<ActionResult<OverrideScoreboard>> GetScoreboard(CancellationToken cancellationToken, int roundId = -1)
        {
            string scoreboard;
            try
            {
                scoreboard = await this._cache.GetOrCreateAsync(roundId, async () =>
                {
                    try
                    {
                        using (var reader = System.IO.File.OpenText(getScoreboardFilePath()))
                        {
                            var scoreboard = await reader.ReadToEndAsync();
                            return scoreboard;
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        throw new ScoreboardNotFoundException();
                    }
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return NotFound();
            }
            return Content(scoreboard);
        }

        /// <summary>
        /// Gets the scoreboard of a given roundId.
        /// The round will be parsed from the JSON.
        /// </summary>
        /// <param name="adminSecret">The admin secret for authenticating the request.</param>
        /// <param name="body">The body of the request.</param>
        /// <returns>The scoreboard of the given roundId.</returns>
        [HttpPost]
        [Route("scoreboard")]
        public async Task<ActionResult> PostScoreboard(string adminSecret, [FromBody] Scoreboard scoreboard, CancellationToken cancellationToken)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                this.logger.LogInformation("Somebody unauthorized tried to update the Scoreboard.");
                return this.Unauthorized();
            }

            Scoreboard? previousScoreboard = null;
            // try
            // {
            using (var reader = System.IO.File.OpenText(getScoreboardFilePath()))
            {
                var text = await reader.ReadToEndAsync();
                var board = JsonSerializer.Deserialize<OverrideScoreboard>(text);
                previousScoreboard = new Scoreboard(
                    board.CurrentRound,
                    board.StartTimestamp,
                    board.EndTimestamp,
                    board.DnsSuffix,
                    board.Services,
                    board.Teams.Select(team =>
                    {
                        return new ScoreboardTeam(
                team.TeamName,
                team.TeamId,
                team.LogoUrl,
                team.CountryCode,
                team.TotalScore,
                team.AttackScore,
                team.DefenseScore,
                team.ServiceLevelAgreementScore,
                        team.ServiceDetails.Select(serviceDetail =>
                        {
                            return new ScoreboardTeamServiceDetails(
                                serviceDetail.ServiceId,
                                serviceDetail.AttackScore,
                                serviceDetail.DefenseScore,
                                serviceDetail.ServiceLevelAgreementScore,
                                serviceDetail.ServiceStatus,
                                serviceDetail.Message
                            );
                        }).ToArray()
                        );
                    }
                 ).ToArray()
                );
            }
            // }
            // catch (Exception)
            // {
            //     throw new ScoreboardNotFoundException();
            // }

            if (previousScoreboard != null)
            {
                previousScoreboard = scoreboard;
            }
            logger.LogWarning($"Prev: {previousScoreboard.Teams.Count()} Current: {scoreboard.Teams.Count()}");
            var bothScoreboardTeams = previousScoreboard.Teams.Zip(scoreboard.Teams, (n, w) => new { previous = n, current = w });
            OverrideScoreboardTeam[] overrideTeams = bothScoreboardTeams.Select(bothTeams =>
            {
                var bothServiceDetails = bothTeams.previous.ServiceDetails.Zip(bothTeams.current.ServiceDetails, (n, w) => new { previous = n, current = w });
                return new OverrideScoreboardTeam(
                    bothTeams.current.TeamName,
                    bothTeams.current.TeamId,
                    bothTeams.current.LogoUrl,
                    bothTeams.current.CountryCode,
                    bothTeams.current.TotalScore,
                    bothTeams.current.AttackScore,
                    bothTeams.current.DefenseScore,
                    bothTeams.current.ServiceLevelAgreementScore,
                    bothServiceDetails.Select(bothServiceDetails =>
                    {
                        return new OverrideScoreboardTeamServiceDetails(
                            bothServiceDetails.current.ServiceId,
                            bothServiceDetails.current.AttackScore,
                            bothServiceDetails.current.DefenseScore,
                            bothServiceDetails.current.ServiceLevelAgreementScore,
                            bothServiceDetails.current.ServiceStatus,
                            bothServiceDetails.current.Message,
                            bothServiceDetails.current.AttackScore - bothServiceDetails.previous.AttackScore,
                            bothServiceDetails.current.DefenseScore - bothServiceDetails.previous.DefenseScore,
                            bothServiceDetails.current.ServiceLevelAgreementScore - bothServiceDetails.previous.ServiceLevelAgreementScore
                        );
                    }
                    ).ToArray(),
                    bothTeams.current.TotalScore - bothTeams.previous.TotalScore,
                    bothTeams.current.AttackScore - bothTeams.previous.AttackScore,
                    bothTeams.current.DefenseScore - bothTeams.previous.DefenseScore,
                    bothTeams.current.ServiceLevelAgreementScore - bothTeams.previous.DefenseScore
                );
            }).ToArray();
            OverrideScoreboard overrideScoreboard = new OverrideScoreboard(
                    scoreboard.CurrentRound,
                    scoreboard.StartTimestamp,
                    scoreboard.EndTimestamp,
                    scoreboard.DnsSuffix,
                    scoreboard.Services,
                     overrideTeams

                );

            using (var createStream = System.IO.File.Create(getScoreboardFilePath()))
            using (var scoreboardRoundFile = System.IO.File.Create(getScoreboardFilePath(scoreboard.CurrentRound)))
            {
                await JsonSerializer.SerializeAsync(createStream, overrideScoreboard, EnoCore.EnoCoreUtil.CamelCaseEnumConverterOptions);
                await JsonSerializer.SerializeAsync(scoreboardRoundFile, overrideScoreboard, EnoCore.EnoCoreUtil.CamelCaseEnumConverterOptions);

            }
            this._cache.InvalidateDefault();
            this.logger.LogDebug("New Scoreboard set.");
            return Ok();
        }

        private string getScoreboardFilePath(long roundId = -1)
        {
            return Path.Combine(Utils.path, "scoreboard" + (roundId < 0 ? "" : roundId) + ".json");
        }
    }

    public class ScoreboardNotFoundException : Exception
    {

    }
}
