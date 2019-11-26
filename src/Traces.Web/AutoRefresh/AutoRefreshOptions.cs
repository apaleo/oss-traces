using System;

namespace Traces.Web.AutoRefresh
{
    public class AutoRefreshOptions
    {
        public string Scheme { get; set; }

        public TimeSpan RefreshBeforeExpiration { get; set; } = TimeSpan.FromMinutes(1);
    }
}