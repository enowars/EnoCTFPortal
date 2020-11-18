namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class VMController : ControllerBase
    {
        [HttpPost]
        [Route("/startvulnbox")]
        public ActionResult StartVulnbox()
        {
            return this.NoContent();
        }

        [HttpPost]
        [Route("/stopvulnbox")]
        public ActionResult StopVulnbox()
        {
            return this.NoContent();
        }
    }
}
