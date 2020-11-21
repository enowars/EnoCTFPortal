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
        public static async Task UpdateTeamVm(IServiceProvider serviceProvider, long teamId, long? serverId, string? ipv4, string? rootPassword, LandingPageVulnboxStatus status)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LandingPageDatabase>();
            await db.UpdateTeamVm(teamId, serverId, ipv4, rootPassword, status);
        }

        public static async Task<LandingPageVulnboxStatus> GetVmStatus(IServiceProvider serviceProvider, long teamId, CancellationToken token)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LandingPageDatabase>();
            return await db.GetVmStatus(teamId, token);
        }

        public static async Task SetVmStatus(IServiceProvider serviceProvider, long teamId, LandingPageVulnboxStatus status, CancellationToken token)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LandingPageDatabase>();
            await db.SetVmStatus(teamId, status, token);
        }

        public async Task UpdateTeamVm(long teamId, long? serverId, string? ipv4, string? rootPassword, LandingPageVulnboxStatus status)
        {
            var dbTeam = await this.context.Teams.Where(t => t.Id == teamId).SingleAsync();
            dbTeam.HetznerServerId = serverId;
            dbTeam.VulnboxStatus = status;
            dbTeam.ExternalAddress = ipv4;
            dbTeam.RootPassword = rootPassword;
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateTeamVmStatus(long teamId, LandingPageVulnboxStatus status)
        {
            var dbTeam = await this.context.Teams.Where(t => t.Id == teamId).SingleAsync();
            dbTeam.VulnboxStatus = status;
            await this.context.SaveChangesAsync();
        }

        public async Task<LandingPageVulnboxStatus> GetVmStatus(long teamId, CancellationToken token)
        {
            return await this.context.Teams.Where(t => t.Id == teamId).Select(t => t.VulnboxStatus).SingleAsync(token);
        }

        public async Task SetVmStatus(long teamId, LandingPageVulnboxStatus status, CancellationToken token)
        {
            var team = await this.context.Teams.Where(t => t.Id == teamId).SingleAsync(token);
            team.VulnboxStatus = status;
            await this.context.SaveChangesAsync(token);
        }
    }
}
