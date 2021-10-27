namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using EnoCore;
    using EnoCore.Configuration;
    using EnoCore.Models;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageBackend.Hetzner;
    using EnoLandingPageCore;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
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

            await this.hetznerApi.Call(teamId, HetznerCloudApiCallType.Create);
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
                        "flagsigningkey",
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
    }
}
