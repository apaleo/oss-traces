using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Traces.Common.Constants;

namespace Traces.Web.Pages
{
    public class Logout : PageModel
    {
        public async Task OnGetAsync(string redirectPath = "/")
        {
            await HttpContext.SignOutAsync();

            Response.Redirect($"{redirectPath}&{ApaleoOneConstants.DisableAccountCheckQueryParameter}=true");
        }
    }
}