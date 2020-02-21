using System.Collections.Generic;
using Traces.Common.Utils;
using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Utils.Converters.Files
{
    public static class TraceFileConverterExtensions
    {
        public static TraceFileItemModel[] ConvertToTraceFileItemModelArray(this TraceFileDto[] dtoArray)
        {
            Check.NotNull(dtoArray, nameof(dtoArray));

            return new List<TraceFileDto>(dtoArray).ConvertAll(model => model.ConvertToTraceFileItemModel()).ToArray();
        }

        public static List<TraceFileItemModel> ConvertToTraceFileItemModelArray(this List<TraceFileDto> dtoArray)
        {
            Check.NotNull(dtoArray, nameof(dtoArray));

            return new List<TraceFileDto>(dtoArray).ConvertAll(model => model.ConvertToTraceFileItemModel());
        }

        public static TraceFileItemModel ConvertToTraceFileItemModel(this TraceFileDto dto)
        {
            Check.NotNull(dto, nameof(dto));

            return new TraceFileItemModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Path = dto.Path,
                Size = dto.Size,
                CreatedBy = dto.CreatedBy,
                MimeType = dto.MimeType,
                PublicId = dto.PublicId,
                TraceId = dto.TraceId
            };
        }

        public static SavedFileItemModel ConvertToSavedFileItemModel(this SavedFileDto dto)
        {
            Check.NotNull(dto, nameof(dto));

            return new SavedFileItemModel
            {
                Data = dto.Data,
                TraceFile = dto.TraceFile.ConvertToTraceFileItemModel()
            };
        }

        public static CreateTraceFileItemModel[] ConvertToCreateTraceFileItemModelArray(this FileToUploadModel[] files, int traceId)
        {
            Check.NotNull(files, nameof(files));

            return new List<FileToUploadModel>(files).ConvertAll(file => file.ConvertToCreateTraceFileItemModel(traceId)).ToArray();
        }

        public static CreateTraceFileItemModel ConvertToCreateTraceFileItemModel(this FileToUploadModel file, int traceId)
        {
            Check.NotNull(file, nameof(file));

            return new CreateTraceFileItemModel
            {
                Data = file.Data,
                Name = file.Name,
                Size = file.Entry.Size,
                MimeType = file.Entry.Type,
                TraceId = traceId
            };
        }

        public static CreateTraceFileDto[] ConvertToCreateTraceFileDtoArray(this CreateTraceFileItemModel[] modelArray)
        {
            Check.NotNull(modelArray, nameof(modelArray));

            return new List<CreateTraceFileItemModel>(modelArray).ConvertAll(model => model.ConvertToCreateTraceFileDto()).ToArray();
        }

        public static CreateTraceFileDto ConvertToCreateTraceFileDto(this CreateTraceFileItemModel model)
        {
            Check.NotNull(model, nameof(model));

            return new CreateTraceFileDto
            {
                Data = model.Data,
                Name = model.Name,
                Size = model.Size,
                MimeType = model.MimeType,
                TraceId = model.TraceId
            };
        }
    }
}
