namespace EnoLandingPageCore.Hetzner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HetznerException : Exception
    {
        public HetznerException()
        {
        }

        public HetznerException(string message)
            : base(message)
        {
        }
    }
}
