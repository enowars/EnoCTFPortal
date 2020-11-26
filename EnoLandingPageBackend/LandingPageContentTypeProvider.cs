namespace EnoLandingPageBackend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.StaticFiles;

    public class LandingPageContentTypeProvider : IContentTypeProvider
    {
        public bool TryGetContentType(string subpath, out string contentType)
        {
            if (subpath.EndsWith(".dll"))
            {
                contentType = "application/x-msdownload";
                return true;
            }

            contentType = string.Empty;
            return false;
        }
    }
}
