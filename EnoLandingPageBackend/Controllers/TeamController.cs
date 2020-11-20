namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageCore;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    [Authorize]
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class TeamController : ControllerBase
    {
        private readonly ILogger<TeamController> logger;
        private readonly LandingPageDatabase db;

        public TeamController(ILogger<TeamController> logger, LandingPageDatabase db)
        {
            this.logger = logger;
            this.db = db;
        }

        [HttpGet]
        public async Task<ActionResult> TeamInfo()
        {
            var team = await this.db.GetTeam(this.GetTeamId());
            this.logger.LogDebug("TeamInfo");
            return this.Ok(new TeamInfo(
                team.Id,
                team.Name,
                null,
                null,
                null,
                null,
                VulnboxStatus.Stopped));
        }
    }
}
