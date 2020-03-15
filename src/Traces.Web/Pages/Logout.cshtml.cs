using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Traces.Web.Pages
{
    public class Logout : PageModel
    {
        public async Task OnGetAsync(string redirectPath = "/")
        {
            Console.WriteLine("====== Signing out");
            await HttpContext.SignOutAsync();

            var decodedUrl = HttpUtility.UrlDecode(redirectPath);

            Response.Redirect(decodedUrl);
        }
    }
}