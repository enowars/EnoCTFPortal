namespace EnoLandingPageCore.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public record TeamsMessage(List<TeamMessage> ConfirmedTeams, List<TeamMessage> RegisteredTeams);

    public record TeamMessage(string Name, long? CtftimeId, string? LogoUrl, string? CountryCode);
}
