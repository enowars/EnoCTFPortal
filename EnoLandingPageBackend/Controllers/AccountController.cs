namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Database;
    using EnoLandingPageCore.Messages;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly LandingPageDatabase db;

        public AccountController(ILogger<AccountController> logger, LandingPageDatabase db)
        {
            this.logger = logger;
            this.db = db;
        }

        [HttpGet]
        public ActionResult Login(string redirectUri) // TODO 404 foo makes ReturnUrl out of this
        {
            return this.Challenge(
                new AuthenticationProperties()
                {
                    RedirectUri = $"/api/account/oauth2redirect?redirectUri={HttpUtility.UrlEncode(redirectUri)}",
                },
                "ctftime.org");
        }

        // All this mess would not be necessary if I could
        // - access my database to create the user
        // - deliver my index.html with a header that contains my JWT
        // in my OnCreatingTicket handler
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult> OAuth2Redirect(string redirectUri)
        {
            this.logger.LogInformation("OAuth2Redirect");
            var authResult = await this.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            long ctftimeId = long.Parse(authResult.Principal!.Claims.Where(c => c.Type == LandingPageClaimTypes.CtftimeId).Single().Value);
            string teamName = authResult.Principal!.Identity!.Name!;

            var team = await this.db.UpdateTeamName(ctftimeId, teamName, this.HttpContext.RequestAborted);

            // TODO create token and put into header

            this.HttpContext.Response.Headers.Add("test2", "test2");
            this.HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
            return this.File("index.html", "text/html");
            // return this.Redirect(redirectUri);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Info()
        {
            var team = await this.db.GetTeamAndVulnbox(this.GetTeamId(), this.HttpContext.RequestAborted);
            this.logger.LogDebug("TeamInfo");
            return this.Ok(new TeamDetailsMessage(
                team.Id,
                team.Confirmed,
                team.Name,
                null, // vpnconfig
                team.Vulnbox.RootPassword,
                team.Vulnbox.ExternalAddress,
                null, // internal ip
                team.Vulnbox.VulnboxStatus));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CheckIn()
        {
            long teamId = this.GetTeamId();
            await this.db.CheckIn(teamId, this.HttpContext.RequestAborted);
            return this.NoContent();
        }
    }
}
