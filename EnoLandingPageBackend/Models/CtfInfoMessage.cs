namespace EnoLandingPageCore.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    // Update with already calculated values
    public record CtfInfoMessage(
        DateTime StartTime,
        long RegistrationCloseOffset,
        long CheckInBeginOffset,
        long CheckInEndOffset);
}
