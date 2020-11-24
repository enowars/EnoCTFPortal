namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Messages;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CTFController : Controller
    {
        private readonly LandingPageSettings settings;

        public CTFController(LandingPageSettings settings)
        {
            this.settings = settings;
        }

        [HttpGet]
        public IActionResult Info()
        {
            return this.Ok(new CtfInfoMessage(
                this.settings.StartTime.ToUniversalTime(),
                this.settings.RegistrationCloseOffset,
                this.settings.CheckInBeginOffset,
                this.settings.CheckInEndOffset));
        }
    }
}
