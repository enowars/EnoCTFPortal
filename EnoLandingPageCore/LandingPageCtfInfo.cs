namespace EnoLandingPageCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public record LandingPageCtfInfo(
        DateTime StartTime,
        long RegistrationCloseHours);
}
