namespace EnoLandingPageCore.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LandingPageTeam
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long Id { get; set; }

        public long? CtftimeId { get; set; }

        public bool Confirmed { get; set; }

        public string Name { get; set; }

        public string? LogoUrl { get; set; }

        public string? UniversityAffiliation { get; set; }

        public string? CountryCode { get; set; }

        public long VulnboxId { get; set; }

        public virtual LandingPageTeamVulnbox Vulnbox { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
