using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Traces.Common;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Web.Models.Files;
using Traces.Web.Services;
using Traces.Web.ViewModels;

namespace Traces.Web.Pages
{
    public class FilesPage : BasePageModel
    {
        private readonly IFileService _fileService;

        public FilesPage(IFileService fileService, IRequestContext requestContext)
            : base(requestContext)
        {
            _fileService = Check.NotNull(fileService, nameof(fileService));
        }

        public async Task<ActionResult> OnGetAsync(string publicId)
        {
            await InitializeContextAsync();

            var result = await _fileService.GetSavedFileFromPublicIdAsync(publicId);

            if (!result.Success)
            {
                return new NotFoundResult();
            }

            var savedFile = result.Result.ValueOrException(new NotImplementedException());

            return File(savedFile.Data, savedFile.TraceFile.MimeType, savedFile.TraceFile.Name);
        }
    }
}