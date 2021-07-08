namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using EnoCore.AttackInfo;
    using Microsoft.Extensions.Logging;
    using EnoLandingPageCore;
    using Microsoft.Extensions.Caching.Memory;
    using EnoLandingPageBackend.Cache;
    using EnoLandingPageBackend.Database;
    using System.Threading;

    /// <summary>
    /// Retrieve the AttackInfo.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class AttackInfoController : ControllerBase
    {
        private readonly ILogger<AttackInfoController> logger;
        private readonly LandingPageSettings settings;
        private AttackCache _cache;

        /// <summary>
        /// Initalizes the AttackInfoController.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <param name="cache"></param>
        public AttackInfoController(ILogger<AttackInfoController> logger, LandingPageSettings settings, AttackCache cache)
        {
            this.logger = logger;
            this.settings = settings;
            this._cache = cache;
        }

        /// <summary>
        /// Gets the current AttackInfo.
        /// </summary>
        /// <returns>The AttackInfo of the current round.</returns>
        [HttpGet]
        public async Task<ActionResult<AttackInfo>> GetCurrentAttackInfo(CancellationToken cancellationToken)
        {
            AttackInfo attackInfo;
            attackInfo = this._cache.TryGetDefault();
            if (attackInfo == null)
            {
                return NotFound();
            }
            return this.Ok(attackInfo);
        }

        /// <summary>
        /// Sets the Attack Info.
        /// </summary>
        /// <param name="adminSecret">The admin secret for authenticating the request.</param>
        /// <param name="attackInfo">The body of the request.</param>
        [HttpPost]
        public async Task<ActionResult> PostScoreboard(string adminSecret, [FromBody] AttackInfo attackInfo, CancellationToken cancellationToken)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                this.logger.LogInformation("Somebody unauthorized tried to update the AttackInfo.");
                return this.Unauthorized();
            }
            // await this.database.SaveattackInfo(attackInfo, cancellationToken);
            this._cache.CreateDefault(attackInfo);
            this.logger.LogDebug("New AttackInfo set.");
            return Ok();
        }
    }

    public class AttackInfoNotFoundException : Exception
    {

    }
}
