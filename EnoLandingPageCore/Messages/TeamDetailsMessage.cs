using EnoLandingPageCore.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnoLandingPageCore.Messages
{
    public record TeamDetailsMessage(
        long Id,
        bool Confirmed,
        string TeamName,
        string? VpnConfig,
        string? RootPassword,
        string? ExternalIpAddress,
        string? InternalIpAddress,
        LandingPageVulnboxStatus? VulnboxStatus);
}
