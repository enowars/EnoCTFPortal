namespace EnoLandingPageCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class LandingPageSettings
    {
        [Required]
        public string Title { get; set; } = "ENOWARS 9";

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public long RegistrationCloseOffset { get; set; }

        [Required]
        public long CheckInBeginOffset { get; set; }

        [Required]
        public long CheckInEndOffset { get; set; }

        [Required]
        public string HetznerCloudApiToken { get; set; }

        [Required]
        public string HetznerVulnboxType { get; set; }

        [Required]
        public string HetznerVulnboxImage { get; set; }

        [Required]
        public string HetznerVulnboxPubkey { get; set; }

        [Required]
        public string HetznerVulnboxLocation { get; set; }

        [Required]
        public string OAuthClientId { get; set; }

        [Required]
        public string OAuthClientSecret { get; set; }

        [Required]
        public string AdminSecret { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
