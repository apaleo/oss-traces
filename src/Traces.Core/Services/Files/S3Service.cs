using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class S3Service : IFileManagerService, System.IDisposable
    {
        private const string BucketName = "oss-traces.apaleo-local";

        private readonly IAmazonS3 _s3Client = new AmazonS3Client(new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.EUWest1,
            ServiceURL = "http://localhost:4572",
            ForcePathStyle = true
        });

        public async Task CreateFileAsync(TraceFile traceFile, MemoryStream data)
        {
            try
            {
                using (var fileTransferUtility = new TransferUtility(_s3Client))
                {
                    await fileTransferUtility.UploadAsync(data, BucketName, traceFile.Path);
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new BusinessValidationException(TextConstants.FileCreateExceptionMessage, e);
            }
        }

        public async Task<MemoryStream> GetFileAsync(TraceFile traceFile)
        {
            try
            {
                GetObjectResponse response = await _s3Client.GetObjectAsync(BucketName, traceFile.Path);
                MemoryStream memoryStream = new MemoryStream();

                using (Stream responseStream = response.ResponseStream)
                {
                    responseStream.CopyTo(memoryStream);
                }

                return memoryStream;
            }
            catch (AmazonS3Exception e)
            {
                throw new BusinessValidationException(TextConstants.FileGetExceptionMessage, e);
            }
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
                throw new BusinessValidationException(TextConstants.FileDeleteExceptionMessage, e);
            }
        }

        public void Dispose()
        {
            _s3Client.Dispose();
        }
    }
}
