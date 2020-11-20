namespace EnoLandingPageCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LandingPageSettings
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public long RegistrationCloseHours { get; set; } = 2;

        public string? OAuthClientId { get; set; } = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");

        public string? OAuthClientSecret { get; set; } = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");

        public string? OAuthAuthorizationEndpoint { get; set; } = Environment.GetEnvironmentVariable("OAUTH_AUTHORIZATION_ENDPOINT");

        public string? OAuthTokenEndpoint { get; set; } = Environment.GetEnvironmentVariable("OAUTH_TOKEN_ENDPOINT");

        public string? OAuthUserInformationEndpoint { get; set; } = Environment.GetEnvironmentVariable("OAUTH_USER_INFORMATION_ENDPOINT");

        public string? OAuthScope { get; set; } = Environment.GetEnvironmentVariable("OAUTH_SCOPE");
    }
}
