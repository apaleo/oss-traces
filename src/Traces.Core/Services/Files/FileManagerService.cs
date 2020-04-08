using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class FileManagerService : IFileManagerService, System.IDisposable
    {
        private const string BucketName = "oss-traces.apaleo-local";
        private readonly ILogger<FileManagerService> _logger;

        private readonly IAmazonS3 _s3Client = new AmazonS3Client(new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.EUWest1,
            ServiceURL = "http://localhost:4572",
            ForcePathStyle = true
        });

        public FileManagerService(ILogger<FileManagerService> logger)
        {
            _logger = logger;
        }

        public async Task CreateFileAsync(TraceFile traceFile, MemoryStream data)
        {
            using (var fileTransferUtility = new TransferUtility(_s3Client))
            {
                await fileTransferUtility.UploadAsync(data, BucketName, traceFile.Path);
            }
        }

        public async Task<MemoryStream> GetFileAsync(TraceFile traceFile)
        {
            GetObjectResponse response = await _s3Client.GetObjectAsync(BucketName, traceFile.Path);
            MemoryStream memoryStream = new MemoryStream();

            using (Stream responseStream = response.ResponseStream)
            {
                responseStream.CopyTo(memoryStream);
            }

            return memoryStream;
        }

        public async Task DeleteFileRangeAsync(IReadOnlyList<TraceFile> traceFiles)
        {
            var keysAndVersions = traceFiles.Select(tf => new KeyVersion
            {
                Key = tf.Path
            }).ToList();

            try
            {
                await _s3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                {
                    BucketName = BucketName,
                    Objects = keysAndVersions
                });
            }
            catch (DeleteObjectsException e)
            {
                PrintDeletionErrorStatus(e);
            }
        }

        public void Dispose()
        {
            _s3Client.Dispose();
        }

        private void PrintDeletionErrorStatus(DeleteObjectsException e)
        {
            var errorResponse = e.Response;
            _logger.LogError("Deleted Count {0}", errorResponse.DeletedObjects.Count);

            _logger.LogError("No. of objects successfully deleted = {0}", errorResponse.DeletedObjects.Count);
            _logger.LogError("No. of objects failed to delete = {0}", errorResponse.DeleteErrors.Count);

            _logger.LogError("Printing error data...");
            foreach (var deleteError in errorResponse.DeleteErrors)
            {
                _logger.LogError("Object Key: {0}\t{1}\t{2}", deleteError.Key, deleteError.Code, deleteError.Message);
            }
        }
    }
}
