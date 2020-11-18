using System;
using System.Collections.Generic;
using System.Text;

namespace EnoLandingPageCore
{
    public enum VulnboxStatus
    {
        Running,
        Stopped
    }

    public record TeamInfo(
        string VpnConfig,
        string RootPassword,
        string? ExternalIpAddress,
        string InternalIpAddress,
        VulnboxStatus VulnboxStatus);
}
