using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnoLandingPageBackend.Database
{
    public class LandingPageDatabaseInitializer
    {
        public static async Task Seed(LandingPageDatabase context)
        {
            if (!(await context.GetTeams(new System.Threading.CancellationToken())).Any())
            {
                await context.GetOrUpdateLandingPageTeam(1, "testTeam", null, null, null, new System.Threading.CancellationToken());
            }
        }
    }
}
