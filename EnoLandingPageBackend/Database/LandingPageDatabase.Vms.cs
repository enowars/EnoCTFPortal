namespace EnoLandingPageBackend.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EnoLandingPageCore.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public partial class LandingPageDatabase
    {
        public static async Task UpdateTeamVulnbox(IServiceProvider serviceProvider, long teamId, long? serverId, string? ipv4, string? rootPassword, LandingPageVulnboxStatus status, CancellationToken token)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LandingPageDatabase>();
            await db.UpdateTeamVulnbox(teamId, serverId, ipv4, rootPassword, status, token);
        }

        public static async Task<LandingPageTeamVulnbox> GetTeamVulnbox(IServiceProvider serviceProvider, long teamId, CancellationToken token)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LandingPageDatabase>();
            return await db.GetTeamVulnbox(teamId, token);
        }

        public async Task UpdateTeamVulnbox(long teamId, long? hetznerServerId, string? ipv4, string? rootPassword, LandingPageVulnboxStatus status, CancellationToken token)
        {
            var dbTeam = await this.context.Vulnboxes
                .Where(t => t.LandingPageTeamId == teamId)
                .SingleAsync(token);
            dbTeam.HetznerServerId = hetznerServerId;
            dbTeam.VulnboxStatus = status;
            dbTeam.ExternalAddress = ipv4;
            dbTeam.RootPassword = rootPassword;
            await this.context.SaveChangesAsync(token);
        }

        private async Task<LandingPageTeamVulnbox> GetTeamVulnbox(long teamId, CancellationToken token)
        {
            return await this.context.Vulnboxes
                .Where(t => t.LandingPageTeamId == teamId)
                .SingleAsync(token);
        }
    }
}
