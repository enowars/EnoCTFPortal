namespace EnoLandingPageBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using EnoCore.Scoreboard;

    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class ScoreboardInfoController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Scoreboard> getScoreboard()
        {
            return this.Ok();
        }
    }
}
