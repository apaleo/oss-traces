using System;
using System.Collections.Generic;
using System.Linq;

namespace Traces.Common.Utils
{
    /// <summary>
    /// Original code from https://github.com/aspnet/EntityFramework/blob/dev/src/Shared/Check.cs.
    /// </summary>
    public static class Check
    {
        public static T NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value is null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T NotNull<T>(T value, string parameterName, string propertyName)
            where T : class
        {
            if (value is null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                NotEmpty(propertyName, nameof(propertyName));

                throw new ArgumentException(GetArgumentPropertyNullMessage(propertyName, parameterName));
            }

            return value;
        }

        public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(GetCollectionArgumentIsEmptyMessage(parameterName));
            }

            return value;
        }

        public static IEnumerable<T> NotEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (!value.Any())
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(GetCollectionArgumentIsEmptyMessage(parameterName));
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            Exception e = null;
            if (value is null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(GetArgumentIsEmptyMessage(parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            return value;
        }

        public static string NullButNotEmpty(string value, string parameterName)
        {
            if (value is string s && s.Length == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(GetArgumentIsEmptyMessage(parameterName));
            }

            return value;
        }

        public static IReadOnlyList<T> HasNoNulls<T>(IReadOnlyList<T> value, string parameterName)
            where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            return value;
        }

        private static string GetArgumentPropertyNullMessage(string propertyName, string parameterName) =>
            $"The property '{propertyName}' of the argument '{parameterName}' cannot be null.";

        private static string GetCollectionArgumentIsEmptyMessage(string parameterName) =>
            $"The collection argument '{parameterName}' must contain at least one element.";

        private static string GetArgumentIsEmptyMessage(string parameterName) =>
            $"The string argument '{parameterName}' cannot be empty.";
    }
}