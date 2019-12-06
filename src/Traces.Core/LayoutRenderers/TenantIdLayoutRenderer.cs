using System.Linq;
using System.Text;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;
using Traces.Common.Constants;

namespace Traces.Core.LayoutRenderers
{
    [LayoutRenderer("tenant-id")]
    public class TenantIdLayoutRenderer : AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;

            if (context == null)
            {
                return;
            }

            string accountCode = context.User.Claims.FirstOrDefault(x => x.Type == ApaleoClaims.AccountCode)?.Value;

            builder.Append(accountCode);
        }
    }
}