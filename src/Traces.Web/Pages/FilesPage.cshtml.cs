using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Traces.Common;
using Traces.Web.Models.File;
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
            _fileService = fileService;
        }

        public async Task<ActionResult> OnGetAsync(string publicId)
        {
            await InitializeContextAsync();

            var result = await _fileService.GetSavedFileFromPublicIdAsync(publicId);

            var savedFile = result.Result.ValueOr(new SavedFileItemModel());

            return File(savedFile.Data, savedFile.TraceFile.MimeType, savedFile.TraceFile.Name);
        }
    }
}