namespace EnoLandingPageBackend
{
    public class Utils
    {
        public const int TeamSubnetBytesLength = 15;

        public static string VulnboxIpAddressForId(long id)
        {
            return $"10.1.{id}.1";
        }

        public static string TeamSubnetForId(long id)
        {
            return $"::ffff:10.1.{id}.0";
        }

        public static bool CanRegisterWithCountry(string[] AllowedCountries, string[] DisallowedCountries, string? country) {
            string? countryCode = country?.toLower();
            if (AllowedCountries != null && AllowedCountries.Length != 0) {
                // Filter by allowed countries!
                if(!AllowedCountries.Contains(countryCode)) {
                    return false;
                }
            }
            if(DisallowedCountries != null && DisallowedCountries.Length != 0) {
                // Filter disallowed countries
                if(DisallowedCountries.Contains(countryCode)) {
                    return false;
                }
            }
            return true;
        }
    }
}
