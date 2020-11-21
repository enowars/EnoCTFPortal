namespace EnoLandingPageCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

#pragma warning disable CS8601 // Possible null reference assignment.
    public class LandingPageSettings
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public long RegistrationCloseOffset { get; set; }

        public long CheckInBeginOffset { get; set; }

        public long CheckInEndOffset { get; set; }

        public string HetznerCloudApiToken { get; set; } = Environment.GetEnvironmentVariable("HETZNER_CLOUD_API_TOKEN");

        public string HetznerVulnboxType { get; set; } = Environment.GetEnvironmentVariable("HETZNER_VULNBOX_TYPE");

        public string HetznerVulnboxImage { get; set; } = Environment.GetEnvironmentVariable("HETZNER_VULNBOX_IMAGE");

        public string HetznerVulnboxPubkey { get; set; } = Environment.GetEnvironmentVariable("HETZNER_VULNBOX_PUBKEY");

        public string HetznerVulnboxLocation { get; set; } = Environment.GetEnvironmentVariable("HETZNER_VULNBOX_LOCATION");

        public string OAuthClientId { get; set; } = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");

        public string OAuthClientSecret { get; set; } = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");

        public string OAuthAuthorizationEndpoint { get; set; } = Environment.GetEnvironmentVariable("OAUTH_AUTHORIZATION_ENDPOINT");

        public string OAuthTokenEndpoint { get; set; } = Environment.GetEnvironmentVariable("OAUTH_TOKEN_ENDPOINT");

        public string OAuthUserInformationEndpoint { get; set; } = Environment.GetEnvironmentVariable("OAUTH_USER_INFORMATION_ENDPOINT");

        public string OAuthScope { get; set; } = Environment.GetEnvironmentVariable("OAUTH_SCOPE");
    }
#pragma warning restore CS8601 // Possible null reference assignment.
}
