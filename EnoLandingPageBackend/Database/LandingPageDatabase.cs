﻿namespace EnoLandingPageBackend.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using EnoLandingPageCore;
    using EnoLandingPageCore.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public partial class LandingPageDatabase
    {
        private readonly ILogger<LandingPageDatabase> logger;
        private readonly LandingPageDatabaseContext context;

        public LandingPageDatabase(ILogger<LandingPageDatabase> logger, LandingPageDatabaseContext databaseContext)
        {
            this.logger = logger;
            this.context = databaseContext;
        }

        public void Migrate()
        {
            var pendingMigrations = this.context.Database.GetPendingMigrations().Count();
            if (pendingMigrations > 0)
            {
                this.logger.LogInformation($"Applying {pendingMigrations} migration(s)");
                this.context.Database.Migrate();
                this.context.SaveChanges();
                this.logger.LogDebug($"Database migration complete");
            }
            else
            {
                this.logger.LogDebug($"No pending migrations");
            }
        }

        public async Task<LandingPageTeam> GetTeamAndVulnbox(long teamId, CancellationToken token)
        {
            return await this.context.Teams
                .Where(t => t.Id == teamId)
                .Include(t => t.Vulnbox)
                .SingleAsync(token);
        }

        public async Task<List<LandingPageTeam>> GetTeams(CancellationToken token)
        {
            return await this.context.Teams
                .ToListAsync(token);
        }

        public async Task<bool> CtftimeTeamExists(long ctftimeId, CancellationToken token)
        {
            return await this.context.Teams
                .AnyAsync(t => t.CtftimeId == ctftimeId, token);
        }

        public async Task<LandingPageTeam> InsertOrUpdateLandingPageTeam(long? ctftimeId, string name, string? logoUrl, string? universityAffiliation, string? countryCode, CancellationToken token, bool? confirmed)
        {
            var dbTeam = await this.context.Teams.Where(t => t.CtftimeId == ctftimeId).SingleOrDefaultAsync(token);
            if (dbTeam == null)
            {
                dbTeam = new LandingPageTeam()
                {
                    CtftimeId = ctftimeId,
                    Confirmed = confirmed ?? false,
                    Name = name,
                    LogoUrl = logoUrl,
                    UniversityAffiliation = universityAffiliation,
                    CountryCode = countryCode,
                    Vulnbox = new LandingPageTeamVulnbox(),
                };
                this.context.Add(dbTeam);
            }
            else
            {
                dbTeam.Name = name;
                dbTeam.CtftimeId = ctftimeId;
                if (confirmed is bool confirmedBool)
                {
                    dbTeam.Confirmed = confirmedBool;
                }

                dbTeam.Name = name;
                dbTeam.LogoUrl = logoUrl;
                dbTeam.UniversityAffiliation = universityAffiliation;
                dbTeam.CountryCode = countryCode;
            }

            await this.context.SaveChangesAsync(token);
            return dbTeam;
        }

        public async Task CheckIn(long teamId, CancellationToken token)
        {
            var dbTeam = await this.context.Teams.Where(t => t.Id == teamId).SingleAsync(token);
            dbTeam.Confirmed = true;
            await this.context.SaveChangesAsync(token);
        }

        public async Task CheckInCtftimeid(long ctftimeId, CancellationToken token)
        {
            var dbTeam = await this.context.Teams.Where(t => t.CtftimeId == ctftimeId).SingleAsync(token);
            dbTeam.Confirmed = true;
            await this.context.SaveChangesAsync(token);
        }

        public async Task<bool> IsCheckedIn(long teamId, CancellationToken token)
        {
            var dbTeam = await this.context.Teams.Where(t => t.Id == teamId).SingleAsync(token);
            return dbTeam.Confirmed;
        }
    }
}
