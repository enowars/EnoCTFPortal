namespace EnoLandingPageBackend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public static class ControllerBaseExtensions
    {
        public static long GetTeamId(this ControllerBase self)
        {
            var ni = self.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(ni, out var teamId))
            {
                return teamId;
            }

            throw new Exception("Invalid NameIdentifier Claim");
        }
    }
}
