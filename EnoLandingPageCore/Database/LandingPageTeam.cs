using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnoLandingPageCore.Database
{
    public class LandingPageTeam
    {
        public LandingPageTeam(long id, long? ctftimeId, bool confirmed, string name)
        {
            this.Id = id;
            this.CtftimeId = ctftimeId;
            this.Confirmed = confirmed;
            this.Name = name;
        }

        public long Id { get; set; }

        public long? CtftimeId { get; set; }

        public bool Confirmed { get; set; }

        public string Name { get; set; }

        public LandingPageVulnboxStatus? VulnboxStatus { get; set; }

        public string? ExternalAddress { get; set; }

        public long? HetznerServerId { get; set; }
    }
}
