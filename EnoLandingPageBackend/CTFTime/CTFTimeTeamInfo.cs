namespace EnoLandingPageBackend.CTFTime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public record CTFTimeTeamInfo(
        string Name,
        string? Country,
        string? Logo);
}
