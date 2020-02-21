using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Optional;
using Traces.Common.Exceptions;
using Traces.Core.Services.Files;
using Traces.Web.Models;
using Traces.Web.Models.Files;
using Traces.Web.Utils.Converters.Files;

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

        public async Task<ResultModel<TraceFileItemModel[]>> CreateTraceFileAsync(CreateTraceFileItemModel[] createTraceFileItemModelArray)
        {
            try
            {
                var createTraceFileDtoArray = createTraceFileItemModelArray.ConvertToCreateTraceFileDtoArray();

                var traceFileDtoArray = await _traceFileService.CreateTraceFileAsync(createTraceFileDtoArray);

                var traceFileItemModel = traceFileDtoArray.ConvertToTraceFileItemModelArray();

                return new ResultModel<TraceFileItemModel[]>
                {
                    Result = traceFileItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(CreateTraceFileAsync)} - Exception while trying to create trace file");

                return new ResultModel<TraceFileItemModel[]>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }

        public async Task<ResultModel<SavedFileItemModel>> GetSavedFileFromPublicIdAsync(string publicId)
        {
            try
            {
                var savedFileDto = await _traceFileService.GetSavedFileFromPublicIdAsync(publicId);

                var savedFileItemModel = savedFileDto.ConvertToSavedFileItemModel();

                return new ResultModel<SavedFileItemModel>
                {
                    Result = savedFileItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(GetSavedFileFromPublicIdAsync)} - Exception while trying to get saved trace file");

                return new ResultModel<SavedFileItemModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }
    }
}
