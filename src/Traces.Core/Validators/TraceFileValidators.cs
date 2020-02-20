using Traces.Core.Models.File;

namespace Traces.Core.Validators
{
    public static class TraceFileValidators
    {
        public static bool IsValid(this CreateTraceFileDto createTraceFileDto)
        {
            if (string.IsNullOrWhiteSpace(createTraceFileDto.Name) ||
                string.IsNullOrWhiteSpace(createTraceFileDto.MimeType) ||
                createTraceFileDto.Size <= 0 ||
                createTraceFileDto.Size > 2097152 ||
                createTraceFileDto.TraceId < 0 ||
                createTraceFileDto.Data == null
            )
            {
                return false;
            }

            return true;
        }
    }
}
