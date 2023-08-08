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
            using (var stream = new MemoryStream())
            {
                await request.FormFile.CopyToAsync(stream);
                stream.Position = 0;
                await fileTransferUtility.UploadAsync(stream, request.BucketName, $"{fileId}{extension}");
            }

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
