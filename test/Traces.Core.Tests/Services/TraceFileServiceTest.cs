using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Core.Models.Files;
using Traces.Core.Repositories;
using Traces.Core.Services.Files;
using Traces.Data.Entities;
using Traces.Testing;
using Xunit;

namespace Traces.Core.Tests.Services
{
    public class TraceServiceFileTest : BaseTest
    {
        private const string TestSubjectId = "TestSubjectId";
        private const string TestTenantId = "TestTenantId";

        private const string TestName = "TestName";
        private const string TestMimeType = "TestMimeType";
        private const int TestSize = 1024;

        private const int TestTraceFileId = 5;
        private const string TestPublicId = "TestPublicId";

        private readonly Mock<ITraceFileRepository> _traceFileRepositoryMock;
        private readonly Mock<IRequestContext> _requestContextMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly ITraceFileService _traceFileService;

        public TraceServiceFileTest()
        {
            _traceFileRepositoryMock = MockRepository.Create<ITraceFileRepository>();
            _requestContextMock = MockRepository.Create<IRequestContext>();
            _fileStorageServiceMock = MockRepository.Create<IFileStorageService>();

            _traceFileService = new TraceFileService(
                _traceFileRepositoryMock.Object,
                _fileStorageServiceMock.Object,
                _requestContextMock.Object);
        }

        [Fact]
        public async Task ShouldUploadFilesAsync()
        {
            var obj = GenerateCreateTraceFileDto();
            var param = new List<CreateTraceFileDto>();
            param.Add(obj);

            _requestContextMock.SetupGet(x => x.SubjectId)
                .Returns(TestSubjectId);
            _requestContextMock.SetupGet(x => x.TenantId)
                .Returns(TestTenantId);

            _fileStorageServiceMock.Setup(
                    x => x.CreateFileAsync(
                        It.Is<TraceFile>(t => t.Name == TestName && IsCorrectPath(t)),
                        It.IsAny<MemoryStream>()))
                .Returns(Task.CompletedTask);

            var result = await _traceFileService.UploadStorageFilesAsync(param);

            result.Count.Should().Be(1);
            result.Should().Contain(t => t.Name == TestName && IsCorrectPath(t));
        }

        [Fact]
        public async Task ShouldFailUploadFilesIfParamIsNotValidAsync()
        {
            var param = new List<CreateTraceFileDto>();

            var results = new List<BusinessValidationException>();
            param.Clear();
            param.Add(GenerateCreateTraceFileDto(name: string.Empty));
            results.Add(
                await Assert.ThrowsAsync<BusinessValidationException>(
                    () => _traceFileService.UploadStorageFilesAsync(param)));

            param.Clear();
            param.Add(GenerateCreateTraceFileDto(mime: string.Empty));
            results.Add(
                await Assert.ThrowsAsync<BusinessValidationException>(
                    () => _traceFileService.UploadStorageFilesAsync(param)));

            param.Clear();
            param.Add(GenerateCreateTraceFileDto(size: AppConstants.MaxFileSizeInBytes + 1));
            results.Add(
                await Assert.ThrowsAsync<BusinessValidationException>(
                    () => _traceFileService.UploadStorageFilesAsync(param)));

            results.Count.Should().Be(3);
            foreach (var businessValidationException in results)
            {
                businessValidationException.Message.Should().Be(TextConstants.CreateTraceFileInvalidErrorMessage);
            }
        }

        [Fact]
        public async Task ShouldDeleteFilesAsync()
        {
            var traceFile = new TraceFile { Id = TestTraceFileId };
            var repoResult = new List<TraceFile>();
            repoResult.Add(traceFile);

            _traceFileRepositoryMock
                .Setup(x => x.GetAllTraceFilesForTenantAsync(It.IsAny<Expression<Func<TraceFile, bool>>>()))
                .ReturnsAsync(repoResult);

            _fileStorageServiceMock.Setup(
                    x => x.DeleteFileRangeAsync(It.Is<IReadOnlyList<TraceFile>>(list => list.Equals(repoResult))))
                .Returns(Task.CompletedTask);

            var result = await _traceFileService.DeleteStorageFilesAsync(repoResult.Select(tf => tf.Id).ToList());

            result.Count.Should().Be(1);
            result.Should().Contain(t => t.Id == TestTraceFileId);
        }

        [Fact]
        public async Task ShouldGetSavedFileFromPublicIdAsync()
        {
            var traceFile = new TraceFile { Id = TestTraceFileId };
            _traceFileRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TraceFile, bool>>>()))
                .ReturnsAsync(true);

            _traceFileRepositoryMock
                .Setup(x => x.GetByPublicIdAsync(It.Is<string>(pid => pid == TestPublicId)))
                .ReturnsAsync(traceFile);

            _fileStorageServiceMock.Setup(x => x.GetFileAsync(It.Is<TraceFile>(t => t.Id == TestTraceFileId)))
                .ReturnsAsync(Array.Empty<byte>());

            var result = await _traceFileService.GetSavedFileFromPublicIdAsync(TestPublicId);

            result.TraceFile.Id.Should().Be(TestTraceFileId);
            result.Data.Length.Should().Be(0);
        }

        [Fact]
        public async Task ShouldFailGetSavedFileFromPublicIdIfPublicIdDoesntExistsAsync()
        {
            _traceFileRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<TraceFile, bool>>>()))
                .ReturnsAsync(false);

            var result = await Assert.ThrowsAsync<BusinessValidationException>(
                () => _traceFileService.GetSavedFileFromPublicIdAsync(TestPublicId));

            result.Message.Should().Be(string.Format(TextConstants.TraceFilePublicIdCouldNotBeFoundErrorMessageFormat, TestPublicId));
        }

        private static CreateTraceFileDto GenerateCreateTraceFileDto(
            string name = TestName,
            long size = TestSize,
            string mime = TestMimeType) =>
            new CreateTraceFileDto
            {
                Data = new MemoryStream(),
                Name = name,
                Size = size,
                MimeType = mime
            };

        private static bool IsCorrectPath(TraceFile tf) =>
            tf.Path.StartsWith($"files/{TestTenantId}/") && tf.Path.EndsWith($"/{TestName}");
    }
}
