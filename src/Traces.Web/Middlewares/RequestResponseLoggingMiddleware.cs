using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Traces.Common.Utils;

namespace Traces.Web.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private static readonly IReadOnlyCollection<string> SensibleHeaders = new[] { "Authorization" };

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = Check.NotNull(next, nameof(next));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        #pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Stream originalRequest = context.Request.Body;

            using (var requestStream = new MemoryStream())
            {
                await context.Request?.Body.CopyToAsync(requestStream);

                requestStream.Seek(0L, SeekOrigin.Begin);
                context.Request.Body = requestStream;

                await LogRequestAsync(context.Request);

                await _next.Invoke(context);

                LogResponse(context.Response, stopwatch.ElapsedMilliseconds);

                stopwatch.Stop();
            }

            context.Request.Body = originalRequest;
        }

        #pragma warning restore CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

        private async Task LogRequestAsync(HttpRequest request)
        {
            var builder = new StringBuilder($"{request.Method} {request.Path}{request.QueryString}{Environment.NewLine}");

            AppendHeaders(builder, request.Headers);

            await AppendBodyAsync(request.Body, builder);

            _logger.LogInformation(builder.ToString());
        }

        private void LogResponse(HttpResponse response, long elapsedMilliseconds)
        {
            var builder = new StringBuilder($"Request finished in {elapsedMilliseconds}ms")
                .AppendLine()
                .AppendLine($"StatusCode: {response.StatusCode}");

            AppendHeaders(builder, response.Headers);

            _logger.LogInformation(builder.ToString());
        }

        private static async Task AppendBodyAsync(Stream body, StringBuilder builder)
        {
            if (body != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    body.Seek(0L, SeekOrigin.Begin);
                    await body.CopyToAsync(memoryStream);
                    body.Seek(0L, SeekOrigin.Begin);

                    string content = Encoding.UTF8.GetString(memoryStream.ToArray());
                    builder.AppendLine(content);
                }
            }
        }

        private static void AppendHeaders(StringBuilder builder, IHeaderDictionary headers)
        {
            foreach (KeyValuePair<string, StringValues> header in headers)
            {
                if (SensibleHeaders.Contains(header.Key))
                {
                    builder.AppendLine($"{header.Key}: [HIDDEN-VALUE]");
                }
                else
                {
                    builder.AppendLine($"{header.Key}: {header.Value}");
                }
            }

            builder.AppendLine();
        }
    }
}