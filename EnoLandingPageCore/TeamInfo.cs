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
        string? VpnConfig,
        string? RootPassword,
        string? ExternalIpAddress,
        string? InternalIpAddress,
        VulnboxStatus? VulnboxStatus);
}
