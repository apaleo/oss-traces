using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Optional;
using Traces.Common.Exceptions;
using Traces.Core.Models.TraceFile;
using Traces.Core.Services.Files;
using Traces.Web.Models;
using Traces.Web.Models.TraceFile;
using Traces.Web.Utils;

namespace Traces.Web.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger _logger;
        private readonly ITraceFileService _traceFileService;

        public FileService(ILogger<FileService> logger, ITraceFileService traceFileService)
        {
            _logger = logger;
            _traceFileService = traceFileService;
        }

        public async Task<ResultModel<TraceFileItemModel>> CreateTraceFileAsync(CreateTraceFileItemModel createTraceFileItemModel)
        {
            try
            {
                var createTraceFileDto = new CreateTraceFileDto
                {
                    Name = createTraceFileItemModel.Name,
                    Size = createTraceFileItemModel.Size,
                    TraceId = createTraceFileItemModel.TraceId,
                    MimeType = createTraceFileItemModel.MimeType
                };

                var traceFileDto = await _traceFileService.CreateTraceFileAsync(createTraceFileDto);

                var traceFileItemModel = traceFileDto.ConvertToTraceFileItemModel();

                return new ResultModel<TraceFileItemModel>
                {
                    Result = traceFileItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(CreateTraceFileAsync)} - Exception while trying to create trace file");

                return new ResultModel<TraceFileItemModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }
    }
}
