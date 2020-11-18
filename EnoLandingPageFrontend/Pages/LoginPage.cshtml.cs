using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EnoLandingPageFrontend.Pages
{
    public class LoginPageModel : PageModel
    {
        public async Task OnGet(string redirectUri)
        {
            await HttpContext.ChallengeAsync("ctftime.org", new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        }
    }
}
