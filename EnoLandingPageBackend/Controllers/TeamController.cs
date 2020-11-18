namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EnoLandingPageCore;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ILogger<TeamController> logger;

        public TeamController(ILogger<TeamController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult TeamInfo()
        {
            return this.Ok(new TeamInfo(
                "VpnConfig",
                "RootPassword",
                "ExternalIpAddress",
                "InternalIpAddress",
                VulnboxStatus.Stopped));
        }
    }
}
