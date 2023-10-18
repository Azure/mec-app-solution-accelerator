using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.MecSolutionAccelerator.Services.Files.Infraestructure;

namespace Microsoft.MecSolutionAccelerator.Services.MinIOInfraestructure
{
    public class MinIOService : IFileStorage
    {
        private readonly MinioConfiguration _configuration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 _s3Client;
        private readonly ILogger<MinIOService> _logger;


        public MinIOService(IOptions<MinioConfiguration> configuration, ILogger<MinIOService> logger)
        {
            _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var config = new AmazonS3Config
            {
                RegionEndpoint = bucketRegion,
                ServiceURL = _configuration.ServiceUrl,
                ForcePathStyle = true
            };
            _s3Client = new AmazonS3Client(_configuration.Username, _configuration.Password, config);
        }

        public async Task<MemoryStream> DownloadFile(string fileName, string bucketName, CancellationToken cancellationToken)
        {
            try
            {
                var requestFile = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };
                using var response = await _s3Client.GetObjectAsync(requestFile);
                var responseStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(responseStream);
                responseStream.Position = 0;

                return responseStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the file.");
                throw;
            }
        }

        public async Task DeleteOlderThanFiles(string bucketName, int hours, CancellationToken cancellationToken)
        {
            try
            {
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = bucketName
                };
                var response = await _s3Client.ListObjectsV2Async(listObjectsRequest);
                var currentTime = DateTime.UtcNow;

                foreach (var s3Object in response.S3Objects)
                {
                    if (s3Object.LastModified < currentTime.AddHours(-hours))
                    {
                        var deleteRequest = new DeleteObjectRequest
                        {
                            BucketName = bucketName,
                            Key = s3Object.Key
                        };
                        await _s3Client.DeleteObjectAsync(deleteRequest);
                    }
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when deleting old files");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unknown encountered on server. Message:'{ex.Message}' when deleting old files");
                throw;
            }
        }

        public async Task<Guid> Handle(IFormFile FormFile, string bucketName, string sourceId, long timestamp, CancellationToken cancellationToken)
        {
            var fileId = Guid.NewGuid();
            await EnsureBucketExistsAsync(bucketName);
            var fileTransferUtility = new TransferUtility(_s3Client);
            var extension = Path.GetExtension(FormFile.FileName);
            var keyName = $"{fileId}{extension}";
            // Create request to upload file with metadata
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = new MemoryStream(),
            };
            fileTransferUtilityRequest.Metadata.Add("x-amz-meta-timestamp", timestamp.ToString());
            fileTransferUtilityRequest.Metadata.Add("x-amz-meta-source-id", sourceId.ToString());


            await FormFile.CopyToAsync(fileTransferUtilityRequest.InputStream);
            fileTransferUtilityRequest.InputStream.Position = 0;
            await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

            return fileId;
        }

        private async Task EnsureBucketExistsAsync(string bucketName)
        {
            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    var response = await _s3Client.PutBucketAsync(putBucketRequest);
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error encountered on server. Message:'{e.Message}' when checking for bucket existence");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown encountered on server. Message:'{e.Message}' when checking for bucket existence");
            }
        }
    }
}