using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Optional;
using Traces.Common.Exceptions;
using Traces.Core.Services.Files;
using Traces.Web.Extensions.Files;
using Traces.Web.Models;
using Traces.Web.Models.Files;

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

        public async Task<ResultModel<SavedFileItemModel>> GetSavedFileFromPublicIdAsync(string publicId)
        {
            try
            {
                var savedFileDto = await _traceFileService.GetSavedFileFromPublicIdAsync(publicId);

                var savedFileItemModel = savedFileDto.ToSavedFileItemModel();

                return new ResultModel<SavedFileItemModel>
                {
                    Result = savedFileItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(FileService)}.{nameof(GetSavedFileFromPublicIdAsync)} - Exception while trying to get saved trace file");

                return new ResultModel<SavedFileItemModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }
    }
}
