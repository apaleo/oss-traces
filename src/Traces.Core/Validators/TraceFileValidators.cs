using Traces.Common.Constants;
using Traces.Core.Models.Files;

namespace Traces.Core.Validators
{
    public static class TraceFileValidators
    {
        public static bool IsValid(this CreateTraceFileDto createTraceFileDto)
        {
            if (string.IsNullOrWhiteSpace(createTraceFileDto.Name) ||
                string.IsNullOrWhiteSpace(createTraceFileDto.MimeType) ||
                createTraceFileDto.Size <= 0 ||
                createTraceFileDto.Size > AppConstants.MaxFileSizeInBytes ||
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
