using System;
using System.Collections.Generic;
using System.Text;

namespace EnoLandingPageCore
{
    public enum VulnboxStatus
    {
        Stopped,
        Running,
    }

    public record LandingPageTeamInfo(
        long Id,
        bool Confirmed,
        string TeamName,
        string? VpnConfig,
        string? RootPassword,
        string? ExternalIpAddress,
        string? InternalIpAddress,
        VulnboxStatus? VulnboxStatus);
}
