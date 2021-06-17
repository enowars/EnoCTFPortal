namespace EnoLandingPageCore.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LandingPageTeamVulnbox
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long Id { get; set; }

        public LandingPageVulnboxStatus VulnboxStatus { get; set; }

        public string? ExternalAddress { get; set; }

        public long? HetznerServerId { get; set; }

        public string? RootPassword { get; set; }

        public long LandingPageTeamId { get; set; }

        public virtual LandingPageTeam LandingPageTeam { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
