using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Traces.Common.Constants;

namespace Traces.Web.Pages
{
    public class HostPageModel : PageModel
    {
        public string AccessToken { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                AccessToken = await HttpContext.GetTokenAsync(SecurityConstants.AccessToken);
            }

            return Page();
        }
    }
}
