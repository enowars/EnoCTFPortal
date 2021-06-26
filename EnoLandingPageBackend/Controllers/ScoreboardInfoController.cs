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
        private readonly LandingPageDatabase database;
        private ScoreboardCache _cache;

        /// <summary>
        /// Initalizes the ScoreboardController.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        public ScoreboardInfoController(ILogger<ScoreboardInfoController> logger, LandingPageSettings settings, LandingPageDatabase db, ScoreboardCache cache)
        {
            this.logger = logger;
            this.settings = settings;
            this.database = db;
            this._cache = cache;
        }

        /// <summary>
        /// Gets the current scoreboard.
        /// </summary>
        /// <returns>The scoreboard of the current round.</returns>
        [HttpGet]
        [Route("scoreboard.json")]
        public async Task<ActionResult<Scoreboard>> GetDefaultScoreboard(CancellationToken cancellationToken)
        {
            Scoreboard scoreboard;
            scoreboard = this._cache.TryGetDefault();
            if (scoreboard == null)
            {
                scoreboard = await this.database.GetCurrentScoreboard(cancellationToken);
                this._cache.CreateDefault(scoreboard);
            }
            if (scoreboard == null)
            {
                return NotFound();
            }
            return this.Ok(scoreboard);
        }

        /// <summary>
        /// Gets the scoreboard of a given roundId.
        /// </summary>
        /// <param name="roundId">Number of the round.</param>
        /// <returns>The scoreboard of the given roundId.</returns>
        [HttpGet]
        [Route("scoreboard{roundId}.json")]
        public async Task<ActionResult<Scoreboard>> GetScoreboard(CancellationToken cancellationToken, int roundId = -1)
        {
            Scoreboard scoreboard;
            try
            {
                scoreboard = await this._cache.GetOrCreateAsync(roundId, async () =>
                {
                    var scoreboard = await this.database.GetScoreboard(roundId, cancellationToken);
                    if (scoreboard == null)
                    {
                        throw new ScoreboardNotFoundException();
                    }
                    return scoreboard;
                });
            }
            catch (ScoreboardNotFoundException)
            {
                return NotFound();
            }
            return this.Ok(scoreboard);
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
            await this.database.SaveScoreboard(scoreboard, cancellationToken);
            this._cache.InvalidateDefault();
            this.logger.LogDebug("New Scoreboard set.");
            return Ok();
        }
    }

    public class ScoreboardNotFoundException : Exception
    {

    }
}
