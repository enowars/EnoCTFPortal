namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Threading.Tasks;
    using EnoCore;
    using EnoCore.Configuration;
    using EnoCore.Models;
    using EnoCore.Models.JsonConfiguration;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageBackend.Hetzner;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Database;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly LandingPageSettings settings;
        private readonly LandingPageDatabase db;
        private readonly ILogger<AdminController> logger;
        private readonly JsonSerializerOptions serializerOptions;
        private readonly HetznerCloudApi hetznerApi;

        public AdminController(LandingPageSettings settings, LandingPageDatabase db, HetznerCloudApi hetznerApi, ILogger<AdminController> logger)
        {
            this.settings = settings;
            this.db = db;
            this.hetznerApi = hetznerApi;
            this.logger = logger;
            this.serializerOptions = new JsonSerializerOptions(EnoCoreUtil.CamelCaseEnumConverterOptions);
            this.serializerOptions.WriteIndented = true;
        }

        [HttpGet]
        public async Task<ActionResult> BootVm(string adminSecret, long teamId)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }

            this.logger.LogDebug($"BootVm({teamId})");
            await this.hetznerApi.Call(teamId, HetznerCloudApiCallType.Create);
            return this.NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> ForgetVm(string adminSecret, long teamId)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }

            this.logger.LogDebug($"ForgetVm({teamId})");
            await this.db.UpdateTeamVulnbox(
                    teamId,
                    null,
                    null,
                    null,
                    LandingPageVulnboxStatus.None,
                    this.HttpContext.RequestAborted);
            return this.NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> CtfJson(string adminSecret)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }

            return this.File(
                JsonSerializer.SerializeToUtf8Bytes(
                    new JsonConfiguration(
                        this.settings.Title,
                        10,
                        3,
                        60,
                        ".bambi.ovh",
                        Utils.TeamSubnetBytesLength,
                        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                        FlagEncoding.Legacy,
                        (await this.db.GetTeams(this.HttpContext.RequestAborted))
                            .Where(t => t.Confirmed)
                            .Select(t => new JsonConfigurationTeam(t.Id, t.Name, Utils.VulnboxIpAddressForId(t.Id), Utils.TeamSubnetForId(t.Id), t.LogoUrl, t.CountryCode))
                            .ToList(),
                        new List<JsonConfigurationService>()),
                    this.serializerOptions),
                "application/force-download",
                "ctf.json");
        }

        [HttpGet]
        public async Task<ActionResult> AddTeam(string adminSecret, long? ctftimeId, string name, string? logoUrl, string? universityAffiliation, string? countryCode)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }

            await this.db.InsertOrUpdateLandingPageTeam(ctftimeId, name, logoUrl, universityAffiliation, countryCode, this.HttpContext.RequestAborted, true);
            return this.NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> CheckInTeam(string adminSecret, long id)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }

            await this.db.CheckInCtftimeid(id, this.HttpContext.RequestAborted);
            return this.NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> ShowTeam(string adminSecret, long id)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }
            return this.File(
                JsonSerializer.SerializeToUtf8Bytes(
                    (await this.db.GetTeamAndVulnbox(id, this.HttpContext.RequestAborted)),
                    this.serializerOptions),
                "application/force-download",
                $"team{id}.json");
        }


        [HttpGet]
        public async Task<ActionResult> LoginAsTeam(string adminSecret, long id)
        {
            if (adminSecret != this.settings.AdminSecret)
            {
                return this.Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, $"{id}"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await this.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
            return this.Content("Open your DevConsole and set use: document.cookie=\".AspNetCore.Cookies=<COOKIEVALUE>;\"\n", "text/plain");
        }
    }
}
