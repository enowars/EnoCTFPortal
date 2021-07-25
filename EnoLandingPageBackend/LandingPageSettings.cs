namespace EnoLandingPageCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // TODO: Schema validation!
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class LandingPageSettings
    {
        [Required]
        public string Title { get; set; } = "Foobar CTF";

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public long RegistrationCloseOffset { get; set; } = 24;

        [Required]
        public long CheckInBeginOffset { get; set; } = 24;

        [Required]
        public long CheckInEndOffset { get; set; } = 2;

        /// <summary>
        /// The duration of the CTF Beginning at Start Time in full Hours.
        /// </summary>
        /// <value></value>
        [Required]
        public long Duration { get; set; } = 10;

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

        public DateTime GetCtfEndTime()
        {
            return this.StartTime.AddHours(this.Duration).ToUniversalTime();
        }

        public DateTime GetRegistrationCloseTime()
        {
            return this.StartTime.AddHours(-this.RegistrationCloseOffset).ToUniversalTime();
        }

        public DateTime GetCheckInCloseTime()
        {
            return this.StartTime.AddHours(-this.CheckInEndOffset).ToUniversalTime();
        }

        public DateTime GetCheckInBeginTime()
        {
            return this.StartTime.AddHours(-this.CheckInBeginOffset).ToUniversalTime();
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
