using System;

namespace Traces.Common.Utils
{
    public static class HerokuUtils
    {
        public static string ConvertConnectionStringIfSet(string herokuConnectionString)
        {
            if (string.IsNullOrWhiteSpace(herokuConnectionString))
            {
                return null;
            }

            var uri = new Uri(herokuConnectionString);

            // userName:password
            var dbCredentials = uri.UserInfo.Split(":");
            if (dbCredentials.Length < 2)
            {
                return null;
            }

            var userName = dbCredentials[0];
            var password = dbCredentials[1];

            // cut away the leading "/"
            var dbName = uri.AbsolutePath.Substring(1);

            return $"Host={uri.Host};Port={uri.Port};Database={dbName};Username={userName};Password={password}";
        }
    }
}