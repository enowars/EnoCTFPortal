using System.IO;

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

        public static string path = Directory.GetCurrentDirectory() + "/enginedata";
    }
}
