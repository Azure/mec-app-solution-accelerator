using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.MecSolutionAccelerator.Services.Files.Commands;
using Microsoft.MecSolutionAccelerator.Services.Files.Configuration;

namespace Microsoft.MecSolutionAccelerator.Services.Files.CommandHandlers
{
    public class DeleteFilesByIDCommandHandler : IRequestHandler<DeleteFilesCommand>
    {
        private readonly MinioConfiguration _configuration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 _s3Client;
        private readonly ILogger<DeleteFilesByIDCommandHandler> _logger;

        public DeleteFilesByIDCommandHandler(IOptions<MinioConfiguration> configuration, ILogger<DeleteFilesByIDCommandHandler> logger)
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

        public async Task<Unit> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var multiObjectDeleteRequest = new DeleteObjectsRequest
                {
                    BucketName = request.bucketName
                };

                foreach (var fileId in request.Ids)
                {
                    // Suponiendo que cada ID corresponde directamente al nombre del archivo
                    // o a una parte del nombre del archivo (por ejemplo, UUID.extension).
                    // Si tu estructura de nombres de archivo es diferente, ajústala adecuadamente.
                    multiObjectDeleteRequest.AddKey($"{fileId}.extension");  // Cambia "extension" por la extensión real si es constante, o ajusta según necesites.
                }

                var response = await _s3Client.DeleteObjectsAsync(multiObjectDeleteRequest);

                return Unit.Value;

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error encountered on server. Message:'{e.Message}' when deleting objects");
                return Unit.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown encountered on server. Message:'{e.Message}' when deleting objects");
                return Unit.Value;
            }
        }
    }
}
