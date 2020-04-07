using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Traces.Core.Models.Files;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public interface ITraceFileService
    {
        Task<IReadOnlyList<TraceFileDto>> CreateTraceFileAsync(List<CreateTraceFileDto> createTraceFileDtos);

        Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId);

        Task<bool> DeleteTraceFileRangeAsync(Expression<Func<TraceFile, bool>> expression);
    }
}
