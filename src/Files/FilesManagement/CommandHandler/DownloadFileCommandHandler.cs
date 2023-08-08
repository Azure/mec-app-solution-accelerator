using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FilesManagement.Commands;
using FilesManagement.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace FilesManagement.CommandHandler
{
    public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, DownloadFileResponse>
    {
        private readonly MinioConfiguration _configuration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 _s3Client;
        private readonly ILogger<DownloadFileCommandHandler> _logger;

        public DownloadFileCommandHandler(IOptions<MinioConfiguration> configuration, ILogger<DownloadFileCommandHandler> logger)
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

        public async Task<DownloadFileResponse> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var requestFile = new GetObjectRequest
                {
                    BucketName = request.BucketName,
                    Key = request.FileName
                };
                using var response = await _s3Client.GetObjectAsync(requestFile);
                var responseStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(responseStream);
                responseStream.Position = 0;

                return new DownloadFileResponse { Stream = responseStream };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the file.");
                throw;
            }
        }
    }


}
