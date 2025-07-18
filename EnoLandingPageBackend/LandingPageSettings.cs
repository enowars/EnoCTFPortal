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

        public string GetHetznerVulnboxLocation(string index)
        {
            Dictionary<string, string> map;
            try {
                map = LoadHetznerVulnboxLocationMap();
            } catch(FileNotFoundException ex) {
                map = new Dictionary<string, string>();
            }
            return map.TryGetValue(index, out var location)
                ? location
                : HetznerVulnboxLocation;
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
