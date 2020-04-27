using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class S3StorageService : IFileStorageService, System.IDisposable
    {
        private readonly IOptions<S3Config> _s3UserConfig;

        private readonly IAmazonS3 _s3Client;

        public S3StorageService(IOptions<S3Config> s3UserConfig, IWebHostEnvironment env)
        {
            _s3UserConfig = s3UserConfig;
            var awsConfig = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_s3UserConfig.Value.Region),
            };

            if (env.IsDevelopment())
            {
                awsConfig.ServiceURL = "http://localhost:4572";
                awsConfig.ForcePathStyle = true;
            }

            _s3Client = new AmazonS3Client(awsConfig);
        }

        public async Task CreateFileAsync(TraceFile traceFile, MemoryStream data)
        {
            try
            {
                using (var fileTransferUtility = new TransferUtility(_s3Client))
                {
                    await fileTransferUtility.UploadAsync(data, _s3UserConfig.Value.BucketName, traceFile.Path);
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
                var response = await _s3Client.GetObjectAsync(_s3UserConfig.Value.BucketName, traceFile.Path);
                var memoryStream = new MemoryStream();

                using (var responseStream = response.ResponseStream)
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
                    BucketName = _s3UserConfig.Value.BucketName,
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
