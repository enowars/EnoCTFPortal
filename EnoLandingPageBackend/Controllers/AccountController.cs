namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using EnoLandingPageBackend.CTFTime;
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
        private readonly LandingPageSettings settings;

        public AccountController(ILogger<AccountController> logger, LandingPageDatabase db, LandingPageSettings settings)
        {
            this.logger = logger;
            this.db = db;
            this.settings = settings;
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
            var teamname = this.HttpContext.User.Identity?.Name;
            if (!long.TryParse(ctftimeIdClaim, out long ctftimeId)
                || teamname == null)
            {
                throw new Exception($"OAuth2 failed: ctftimeid={ctftimeIdClaim} teamname={teamname} claims={this.HttpContext.User.Claims.Count()}");
            }

            if (DateTime.UtcNow > this.settings.StartTime.AddHours(-this.settings.RegistrationCloseOffset).ToUniversalTime() &&
                !await this.db.CtftimeTeamExists(ctftimeId, this.HttpContext.RequestAborted))
            {
                return this.Redirect("/registrationclosed");
            }

            CTFTimeTeamInfo? info = null;
            try
            {
                info = await CTFTime.GetTeamInfo(ctftimeId, this.HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                this.logger.LogError($"CTFtime failed to deliver info for ctftime id {ctftimeId}\n{e.StackTrace}");
            }

            try
            {
                if(!Utils.CanRegisterWithCountry(this.settings.AllowedCountries, this.settings.DisallowedCountries, country)) {
                    throw new Exception($"Country {country} is not allowed");
                }
            }catch(Exception e) 
            {
                this.logger.LogError($"Country is not allowed: {ctftimeId}\n{e.StackTrace}");
                return this.Redirect("/registrationprohibited");             
            }

            var team = await this.db.InsertOrUpdateLandingPageTeam(ctftimeId, teamname, info?.Logo, null, info?.Country, this.HttpContext.RequestAborted, null);
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
            var team = await this.db.GetTeamAndVulnbox(this.GetTeamId(), this.HttpContext.RequestAborted).Where(t => Utils.CanRegisterWithCountry(this.settings.AllowedCountries, this.settings.DisallowedCountries, t.CountryCode));
            return this.Ok(new TeamDetailsMessage(
                team.Id,
                team.Confirmed,
                team.Name,
                System.IO.File.Exists($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}teamdata{Path.DirectorySeparatorChar}team{team.Id}{Path.DirectorySeparatorChar}client.conf"),
                team.Vulnbox.RootPassword,
                team.Vulnbox.ExternalAddress,
                Utils.VulnboxIpAddressForId(team.Id), // internal ip
                team.Vulnbox.VulnboxStatus));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> VpnConfig()
        {
            var team = await this.db.GetTeamAndVulnbox(this.GetTeamId(), this.HttpContext.RequestAborted).Where(t => Utils.CanRegisterWithCountry(this.settings.AllowedCountries, this.settings.DisallowedCountries, t.CountryCode));
            var config = System.IO.File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}teamdata{Path.DirectorySeparatorChar}team{team.Id}{Path.DirectorySeparatorChar}client.conf");
            var contentType = "application/force-download";
            return this.File(Encoding.ASCII.GetBytes(config), contentType, "client.ovpn");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> WireguardConfig()
        {
            var team = await this.db.GetTeamAndVulnbox(this.GetTeamId(), this.HttpContext.RequestAborted).Where(t => Utils.CanRegisterWithCountry(this.settings.AllowedCountries, this.settings.DisallowedCountries, t.CountryCode));
            if (this.settings.StartTime.ToUniversalTime() > DateTime.UtcNow || !await this.db.IsCheckedIn(team.Id, this.HttpContext.RequestAborted))
            {
                return this.Forbid();
            }

            var config = System.IO.File.ReadAllText($"{LandingPageBackendUtil.TeamDataDirectory}{Path.DirectorySeparatorChar}teamdata{Path.DirectorySeparatorChar}team{team.Id}{Path.DirectorySeparatorChar}wireguard.conf");
            var contentType = "application/force-download";
            return this.File(Encoding.ASCII.GetBytes(config), contentType, "wireguard.conf");
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CheckIn()
        {
            long teamId = this.GetTeamId();
            if (DateTime.UtcNow > this.settings.StartTime.AddHours(-this.settings.CheckInEndOffset).ToUniversalTime())
            {
                return this.BadRequest("Checkin is already over.");
            }

            if (this.settings.StartTime.AddHours(-this.settings.CheckInBeginOffset).ToUniversalTime() > DateTime.UtcNow)
            {
                return this.BadRequest("Checkin has not yet begun.");
            }

            await this.db.CheckIn(teamId, this.HttpContext.RequestAborted);
            return this.Ok();
        }
    }
}
