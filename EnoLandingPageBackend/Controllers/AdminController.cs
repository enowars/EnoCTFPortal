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

        public AdminController(LandingPageSettings settings, LandingPageDatabase db, ILogger<AdminController> logger)
        {
            this.settings = settings;
            this.db = db;
            this.logger = logger;
            this.serializerOptions = new JsonSerializerOptions(EnoCoreUtil.CamelCaseEnumConverterOptions);
            this.serializerOptions.WriteIndented = true;
        }

        [HttpGet]
        public async Task<ActionResult> CtfJson()
        {
            return this.File(
                JsonSerializer.SerializeToUtf8Bytes(
                    new JsonConfiguration(
                        this.settings.Title,
                        10,
                        3,
                        180,
                        ".bambi.ovh",
                        16,
                        "flagsigningkey",
                        FlagEncoding.Legacy,
                        (await this.db.GetTeams(this.HttpContext.RequestAborted))
                            .Where(t => t.Confirmed)
                            .Select(t => new JsonConfigurationTeam(t.Id, t.Name, $"10.0.0.{t.Id}", $"::ffff:10.0.0.{t.Id}", t.LogoUrl, t.CountryCode))
                            .ToList(),
                        new List<JsonConfigurationService>()),
                    this.serializerOptions),
                "application/force-download",
                "ctf.json");
        }
    }
}
