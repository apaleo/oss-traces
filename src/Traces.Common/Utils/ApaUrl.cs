using System;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Traces.Common.Utils
{
    public static class ApaUrl
    {
        public static string ExtractQueryParameterFromManager(NavigationManager navigationManager, string parameterKey)
        {
            var uri = new Uri(navigationManager.Uri);

            var queryNameValue = HttpUtility.ParseQueryString(uri.Query);

            return queryNameValue[parameterKey];
        }
    }
}