namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using EnoCore.Scoreboard;

    /// <summary>
    /// Retrieve the scoreboard.
    /// If you change this controller you will also have to change the static hosting variant.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class ScoreboardInfoController : ControllerBase
    {

        /// <summary>
        /// Gets the current scoreboard.
        /// </summary>
        /// <returns>The scoreboard of the current round.</returns>
        [HttpGet]
        [Route("scoreboard.json")]
        public ActionResult<Scoreboard> GetDefaultScoreboard()
        {
            return this.Ok();
        }

        /// <summary>
        /// Gets the scoreboard of a given roundId.
        /// </summary>
        /// <param name="roundId">Number of the round.</param>
        /// <returns>The scoreboard of the given roundId.</returns>
        [HttpGet]
        [Route("scoreboard{roundId}.json")]
        public ActionResult<Scoreboard> GetScoreboard(int roundId = -1)
        {
            return this.Ok();
        }

        /// <summary>
        /// Gets the scoreboard of a given roundId.
        /// </summary>
        /// <param name="roundId">Number of the round.</param>
        /// <returns>The scoreboard of the given roundId.</returns>
        [HttpPost]
        [Route("scoreboard{roundId}.json")]
        public ActionResult<Scoreboard> PostScoreboard(int roundId = -1)
        {
            return this.Ok();
        }
    }
}
