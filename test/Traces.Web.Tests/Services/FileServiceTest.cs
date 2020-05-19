using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Optional.Unsafe;
using Traces.Common.Exceptions;
using Traces.Core.Models.Files;
using Traces.Core.Services.Files;
using Traces.Testing;
using Traces.Web.Services;
using Xunit;

namespace Traces.Web.Tests.Services
{
    public class FileServiceTest : BaseTest
    {
        private readonly Mock<ITraceFileService> _traceFileServiceMock;
        private readonly Mock<ILogger<FileService>> _loggerMock = new Mock<ILogger<FileService>>();
        private readonly IFileService _fileService;

        public FileServiceTest()
        {
            _traceFileServiceMock = MockRepository.Create<ITraceFileService>();

            _fileService = new FileService(_loggerMock.Object, _traceFileServiceMock.Object);
        }

        [Fact]
        public async Task GetSavedFileFromPublicIdAsync()
        {
            const string publicId = "TestPublicId";

            _traceFileServiceMock.Setup(x => x.GetSavedFileFromPublicIdAsync(It.Is<string>(v => v == publicId)))
                .ReturnsAsync(new SavedFileDto { TraceFile = new TraceFileDto { Id = 1 } });

            var completeResult = await _fileService.GetSavedFileFromPublicIdAsync(publicId);

            completeResult.Should().NotBeNull();
            completeResult.Success.Should().BeTrue();
            completeResult.ErrorMessage.HasValue.Should().BeFalse();
            completeResult.Result.HasValue.Should().BeTrue();

            var modelResult = completeResult.Result.ValueOrFailure();
            modelResult.TraceFile.Id.Should().Be(1);
        }

        [Fact]
        public async Task ShouldNotBeAbleToGetSavedFileFromPublicIdIfExceptionIsThrownAsync()
        {
            const string exceptionMessage = "Nop nop nop";
            const string publicId = "TestPublicId";

            _traceFileServiceMock.Setup(x => x.GetSavedFileFromPublicIdAsync(It.Is<string>(v => v == publicId)))
                .ThrowsAsync(new BusinessValidationException(exceptionMessage));

            var completeResult = await _fileService.GetSavedFileFromPublicIdAsync(publicId);

            completeResult.Should().NotBeNull();
            completeResult.Success.Should().BeFalse();
            completeResult.Result.HasValue.Should().BeFalse();
            completeResult.ErrorMessage.HasValue.Should().BeTrue();

            var errorMessage = completeResult.ErrorMessage.ValueOrFailure();
            errorMessage.Should().Be(exceptionMessage);
        }
    }
}
