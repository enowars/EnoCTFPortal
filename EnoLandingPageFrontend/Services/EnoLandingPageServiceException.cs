using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnoLandingPageFrontend.Services
{
    public class EnoLandingPageServiceException : Exception
    {
        public EnoLandingPageServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
