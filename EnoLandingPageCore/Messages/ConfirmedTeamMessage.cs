using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnoLandingPageCore.Messages
{
    public record ConfirmedTeamMessage(string Name, long? CtftimeId);
}
