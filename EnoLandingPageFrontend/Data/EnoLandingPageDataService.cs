using EnoLandingPageCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Data
{
    public class EnoLandingPageDataService
    {
        public async Task<TeamInfo> GetTeamInfo()
        {
            return new TeamInfo(null, null, null, null, VulnboxStatus.Running);
        }
    }
}
