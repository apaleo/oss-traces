using System;
using Optional;

namespace Traces.Common.Extensions
{
    public static class OptionExtensions
    {
#pragma warning disable S3626 // Remove redundant jump
        public static T ValueOrException<T>(this Option<T> option, Exception exception) =>
            option.Match(
                t => t,
                () => throw exception);
#pragma warning restore S3626
    }
}