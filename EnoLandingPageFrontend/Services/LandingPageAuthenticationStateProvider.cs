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
        private readonly ILogger<LandingPageDataApiService> logger;
        private readonly LandingPageDataApiService lpService;

        public LandingPageAuthenticationStateProvider(ILogger<LandingPageDataApiService> logger, LandingPageDataApiService lpService)
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
                    new Claim(ClaimTypes.Name, teamInfo.TeamName),
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Server Auth");
                this.logger.LogInformation($"{nameof(LandingPageAuthenticationStateProvider)} returning authorized ({teamInfo.Id})");
                return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
            }
            catch
            {
                this.logger.LogInformation($"{nameof(LandingPageAuthenticationStateProvider)} returning unauthorized");
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }
    }
}
