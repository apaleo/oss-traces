namespace Traces.Common.Utils
{
    public static class FileSizeUnitConverter
    {
        public static double ConvertBytesToMebibytes(long bytes) => bytes / 1024.0 / 1024.0;
    }
}
