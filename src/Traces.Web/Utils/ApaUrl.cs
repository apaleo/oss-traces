using System;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Traces.Web.Utils
{
    public static class ApaUrl
    {
        public static string ExtractQueryParameterFromManager(NavigationManager navigationManager, string parameterKey)
        {
            var uri = new Uri(navigationManager.Uri);

            var queryNameValue = HttpUtility.ParseQueryString(uri.Query);

            return queryNameValue[parameterKey];
        }

        public static bool HasQueryParams(string path)
        {
            var uri = new Uri(path);

            var queryNameValue = HttpUtility.ParseQueryString(uri.Query);

            return queryNameValue.HasKeys();
        }
    }
}