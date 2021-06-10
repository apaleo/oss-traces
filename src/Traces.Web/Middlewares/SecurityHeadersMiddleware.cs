using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Traces.Web.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Content-Security-Policy", new StringValues(
                "frame-ancestors https://*.apaleo.com;"));

            await _next(context);
        }
#pragma warning restore CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
    }
}