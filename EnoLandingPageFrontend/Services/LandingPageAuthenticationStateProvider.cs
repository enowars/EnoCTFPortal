using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class LandingPageAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<EnoLandingPageService> logger;
        private readonly EnoLandingPageService lpService;

        public LandingPageAuthenticationStateProvider(ILogger<EnoLandingPageService> logger, EnoLandingPageService lpService)
        {
            this.logger = logger;
            this.lpService = lpService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var teamInfo = await this.lpService.GetTeamInfo();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, $"{teamInfo.Id}"),
                    new Claim(ClaimTypes.Name, teamInfo.TeamName + "asdf"),
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Server Auth");
                return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.ToString());
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }
    }
}
