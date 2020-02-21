namespace Traces.Web.Utils.Converters
{
    public static class UnitConverter
    {
        public static double ByteToMIB(long bytes) => bytes / 1024.0 / 1024.0;
    }
}
