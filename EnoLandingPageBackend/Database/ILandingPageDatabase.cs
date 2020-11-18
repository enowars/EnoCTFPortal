namespace EnoLandingPageBackend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ILandingPageDatabase
    {
        void Migrate();

        Task UpdateTeam(long? ctftimeId, string name);
    }
}
