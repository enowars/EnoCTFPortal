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

    public record TeamInfo(
        long Id,
        string TeamName,
        string? VpnConfig,
        string? RootPassword,
        string? ExternalIpAddress,
        string? InternalIpAddress,
        VulnboxStatus? VulnboxStatus);
}
