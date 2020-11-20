namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EnoLandingPageCore;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CTFController : Controller
    {
        private LandingPageSettings settings;

        public CTFController(LandingPageSettings settings)
        {
            this.settings = settings;
        }

        public IActionResult Info()
        {
            return this.Ok(new LandingPageCtfInfo(this.settings.StartTime, this.settings.RegistrationCloseHours));
        }
    }
}
