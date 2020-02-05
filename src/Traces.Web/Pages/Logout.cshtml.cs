using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Traces.Common.Constants;
using Traces.Web.Utils;

namespace Traces.Web.Pages
{
    public class Logout : PageModel
    {
        public async Task OnGetAsync(string redirectPath = "/")
        {
            await HttpContext.SignOutAsync();

            var decodedUrl = HttpUtility.UrlDecode(redirectPath);

            if (ApaUrl.HasQueryParams(decodedUrl))
            {
                Response.Redirect($"{decodedUrl}&{ApaleoOneConstants.DisableAccountCheckQueryParameter}=true");
            }
            else
            {
                Response.Redirect(decodedUrl);
            }
        }
    }
}