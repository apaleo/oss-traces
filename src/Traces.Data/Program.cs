namespace Traces.Data
{
    /// <summary>
    /// This is class is needed as workaround for https://github.com/dotnet/cli/issues/2645.
    /// More info here: https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#targeting-class-library-projects-is-not-supported.
    /// </summary>
    public static class Program
    {
        public static void Main()
        {
            // This is class is needed as workaround for https://github.com/dotnet/cli/issues/2645.
        }
    }
}