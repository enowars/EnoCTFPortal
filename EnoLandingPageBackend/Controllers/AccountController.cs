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

        [HttpGet]
        public async Task<ActionResult> OAuth2Redirect(string redirectUri)
        {
            var ctftimeIdClaim = this.HttpContext.User.FindFirst(LandingPageClaimTypes.CtftimeId)?.Value;
            ctftimeIdClaim = "1438";
            var teamname = this.HttpContext.User.Identity?.Name;
            if (!long.TryParse(ctftimeIdClaim, out long ctftimeId)
                || teamname == null)
            {
                throw new Exception("OAuth2 failed");
            }

            var team = await this.db.UpdateTeam(ctftimeId, teamname, this.HttpContext.RequestAborted);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, $"{team.Id}"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await this.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
            return this.Redirect(redirectUri);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Info()
        {
            var team = await this.db.GetTeam(this.GetTeamId(), this.HttpContext.RequestAborted);
            this.logger.LogDebug("TeamInfo");
            return this.Ok(new LandingPageTeamInfo(
                team.Id,
                team.Confirmed,
                team.Name,
                null,
                null,
                null,
                null,
                VulnboxStatus.Stopped));
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
