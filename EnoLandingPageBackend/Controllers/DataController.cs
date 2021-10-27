namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Messages;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly LandingPageSettings settings;
        private readonly LandingPageDatabase db;
        private readonly ILogger logger;

        public DataController(LandingPageSettings settings, LandingPageDatabase db, ILogger<DataController> logger)
        {
            this.settings = settings;
            this.db = db;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult CtfInfo()
        {
            return this.Ok(new CtfInfoMessage(
                this.settings.StartTime.ToUniversalTime(),
                this.settings.EndTime.ToUniversalTime(),
                this.settings.RegistrationCloseOffset,
                this.settings.CheckInBeginOffset,
                this.settings.CheckInEndOffset));
        }

        [HttpGet]
        public async Task<IActionResult> Teams()
        {
            var teams = await this.db.GetTeams(this.HttpContext.RequestAborted);
            return this.Json(
                new TeamsMessage(
                    teams.Where(t => t.Confirmed).Select(t => new TeamMessage(t.Name, t.CtftimeId, t.LogoUrl, t.CountryCode)).ToList(),
                    teams.Where(t => !t.Confirmed).Select(t => new TeamMessage(t.Name, t.CtftimeId, t.LogoUrl, t.CountryCode)).ToList()));
        }

        [HttpGet]
        public async Task<IActionResult> IPs()
        {
            var teams = (await this.db.GetTeams(this.HttpContext.RequestAborted))
                    .Where(t => t.Confirmed)
                    .Select(t => Utils.VulnboxIpAddressForId(t.Id));

            return this.File(
                Encoding.ASCII.GetBytes(string.Join("\n", teams)),
                "application/force-download",
                "ips.txt");
        }
    }
}
