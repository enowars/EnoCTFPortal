namespace EnoLandingPageCore.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public record CtfInfoMessage(
        DateTime StartTime,
        long RegistrationCloseHours);
}
