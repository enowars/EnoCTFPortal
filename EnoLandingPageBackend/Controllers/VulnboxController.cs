namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageBackend.Hetzner;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Hetzner;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class VulnboxController : ControllerBase
    {
        private readonly ILogger<VulnboxController> logger;
        private readonly HetznerCloudApi hetznerApi;
        private readonly LandingPageSettings settings;
        private readonly LandingPageDatabase db;

        public VulnboxController(ILogger<VulnboxController> logger, HetznerCloudApi hetznerApi, LandingPageSettings settings, LandingPageDatabase db)
        {
            this.logger = logger;
            this.hetznerApi = hetznerApi;
            this.settings = settings;
            this.db = db;
        }

        [HttpPost]
        public async Task<ActionResult> StartVulnbox()
        {
            long teamId = this.GetTeamId();
            this.logger.LogInformation($"StartVulnbox {teamId}");
            if (this.settings.StartTime.ToUniversalTime() > DateTime.UtcNow || !await this.db.IsCheckedIn(teamId, this.HttpContext.RequestAborted))
            {
                return this.Forbid();
            }

            try
            {
                await this.hetznerApi.Call(teamId, HetznerCloudApiCallType.Create);
            }
            catch (ServerExistsException)
            {
                return this.UnprocessableEntity($"{nameof(ServerExistsException)}");
            }
            catch (OtherRequestRunningException)
            {
                return this.UnprocessableEntity($"{nameof(OtherRequestRunningException)}");
            }

            return this.NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> ResetVulnbox()
        {
            long teamId = this.GetTeamId();
            this.logger.LogInformation($"ResetVulnbox {teamId}");
            if (this.settings.StartTime.ToUniversalTime() > DateTime.UtcNow || !await this.db.IsCheckedIn(teamId, this.HttpContext.RequestAborted))
            {
                return this.Forbid();
            }

            await this.hetznerApi.Call(teamId, HetznerCloudApiCallType.Reset);
            return this.NoContent();
        }
    }
}
