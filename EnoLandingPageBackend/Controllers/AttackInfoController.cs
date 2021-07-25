namespace EnoLandingPageBackend.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using EnoCore.AttackInfo;
    using Microsoft.Extensions.Logging;
    using EnoLandingPageCore;
    using EnoLandingPageBackend.Cache;
    using System.Threading;
    using System.Text.Json;

    /// <summary>
    /// Retrieve the AttackInfo.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    [Produces("application/json")]
    public class AttackInfoController : ControllerBase
    {
        private readonly ILogger<AttackInfoController> logger;
        private readonly LandingPageSettings settings;
        private CustomMemoryCache<string> _cache;

        /// <summary>
        /// Initalizes the AttackInfoController.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <param name="cache"></param>
        public AttackInfoController(ILogger<AttackInfoController> logger, LandingPageSettings settings, CustomMemoryCache<string> cache)
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
        public ActionResult<AttackInfo> GetCurrentAttackInfo(CancellationToken cancellationToken)
        {
            string attackInfo;
            attackInfo = this._cache.TryGetDefault();
            if (attackInfo == null)
            {
                return NotFound();
            }
            return Content(attackInfo, "application/json");
        }

        /// <summary>
        /// Sets the Attack Info.
        /// </summary>
        /// <param name="adminSecret">The admin secret for authenticating the request.</param>
        /// <param name="attackInfo">The body of the request.</param>
        [HttpPost]
        public ActionResult PostScoreboard(string adminSecret, [FromBody] AttackInfo attackInfo, CancellationToken cancellationToken)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                this.logger.LogInformation("Somebody unauthorized tried to update the AttackInfo.");
                return this.Unauthorized();
            }
            this._cache.CreateDefault(JsonSerializer.Serialize(attackInfo, EnoCore.EnoCoreUtil.CamelCaseEnumConverterOptions));
            this.logger.LogDebug("New AttackInfo set.");
            return Ok();
        }
    }

    public class AttackInfoNotFoundException : Exception
    {

    }
}
