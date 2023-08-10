using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using FilesManagement.Commands;
using FilesManagement.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace FilesManagement.CommandHandler
{
    public class UploadNewFileCommandHandler : IRequestHandler<UploadNewFileCommand, Guid>
    {
        private readonly MinioConfiguration _configuration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 _s3Client;

        public UploadNewFileCommandHandler(IOptions<MinioConfiguration> configuration)
        {
            _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
            var config = new AmazonS3Config
            {
                RegionEndpoint = bucketRegion,
                ServiceURL = _configuration.ServiceUrl,
                ForcePathStyle = true
            };
            _s3Client = new AmazonS3Client(_configuration.Username, _configuration.Password, config);
        }

        public async Task<Guid> Handle(UploadNewFileCommand request, CancellationToken cancellationToken)
        {
            var fileId = Guid.NewGuid();


            await EnsureBucketExistsAsync(request.BucketName);
            var fileTransferUtility = new TransferUtility(_s3Client);
            var extension = Path.GetExtension(request.FormFile.FileName);
            var keyName = $"{fileId}{extension}";
            // Create request to upload file with metadata
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = request.BucketName,
                Key = keyName,
                InputStream = new MemoryStream(),
            };
            fileTransferUtilityRequest.Metadata.Add("x-amz-meta-timestamp", request.Timestamp.ToString());
            fileTransferUtilityRequest.Metadata.Add("x-amz-meta-source-id", request.SourceId.ToString());


            await request.FormFile.CopyToAsync(fileTransferUtilityRequest.InputStream);
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
