namespace EnoLandingPageCore.Hetzner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ServerExistsException : HetznerException
    {
        public ServerExistsException(string message)
            : base(message)
        {
        }
    }
}
