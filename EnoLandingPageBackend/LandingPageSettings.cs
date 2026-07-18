namespace EnoLandingPageCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class LandingPageSettings
    {
        [Required]
        public string Title { get; set; } = "BambiCTF";

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
        public string HetznerVulnboxLocationMapPath { get; set; } = "/app/data/vulnboxes.json";

        private Dictionary<string, string> LoadHetznerVulnboxLocationMap()
        {
            if (!File.Exists(HetznerVulnboxLocationMapPath)) {
                throw new FileNotFoundException($"JSON file not found: {HetznerVulnboxLocationMapPath}");
            }
            var json = File.ReadAllText(HetznerVulnboxLocationMapPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
                         ?? new Dictionary<string, string>();
        }

        private static readonly char[] LocationSeparators = new[] { ' ', '\t', ',' };

        /// <summary>
        /// The locations a vulnbox may be placed in, in the order they should be tried.
        /// A team listed in the location map is pinned to that single location; otherwise the
        /// (possibly multi-valued) HetznerVulnboxLocation setting is used, e.g. "fsn1 nbg1 hel1".
        /// </summary>
        public IReadOnlyList<string> GetHetznerVulnboxLocations(string index)
        {
            Dictionary<string, string> map;
            try {
                map = LoadHetznerVulnboxLocationMap();
            } catch(FileNotFoundException) {
                map = new Dictionary<string, string>();
            }
            if (map.TryGetValue(index, out var location))
            {
                return new[] { location };
            }

            var locations = HetznerVulnboxLocation.Split(
                LocationSeparators,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (locations.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(HetznerVulnboxLocation)} does not contain any location");
            }

            return locations;
        }

        [Required]
        public string OAuthClientId { get; set; }

        [Required]
        public string OAuthClientSecret { get; set; }

        [Required]
        public string AdminSecret { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
