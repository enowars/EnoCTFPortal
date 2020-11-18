namespace EnoLandingPageBackend.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EnoLandingPageBackend.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class LandingPageDatabase : ILandingPageDatabase
    {
        private readonly ILogger<LandingPageDatabase> logger;
        private readonly LandingPageDatabaseContext databaseContext;

        public LandingPageDatabase(ILogger<LandingPageDatabase> logger, LandingPageDatabaseContext databaseContext)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
        }

        public void Migrate()
        {
            var pendingMigrations = this.databaseContext.Database.GetPendingMigrations().Count();
            if (pendingMigrations > 0)
            {
                this.logger.LogInformation($"Applying {pendingMigrations} migration(s)");
                this.databaseContext.Database.Migrate();
                this.databaseContext.SaveChanges();
                this.logger.LogDebug($"Database migration complete");
            }
            else
            {
                this.logger.LogDebug($"No pending migrations");
            }
        }

        public async Task UpdateTeam(long? ctftimeId, string name)
        {
            var dbTeam = await this.databaseContext.Teams.Where(t => t.CtftimeId == ctftimeId).SingleOrDefaultAsync();
            if (dbTeam == null)
            {
                dbTeam = new LandingPageTeam(
                    0,
                    ctftimeId,
                    false,
                    name);
            }
            else
            {
                dbTeam.Name = name;
            }

            dbTeam.CtftimeId = ctftimeId;
            dbTeam.Name = name;
        }
    }
}
