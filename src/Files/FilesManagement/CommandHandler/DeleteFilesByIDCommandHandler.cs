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
                // 1. Listar todos los archivos
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = request.bucketName
                };
                var response = await _s3Client.ListObjectsV2Async(listObjectsRequest);

                // Tomar la fecha y hora actual
                var currentTime = DateTime.UtcNow;

                foreach (var s3Object in response.S3Objects)
                {
                    // 2. Filtrar aquellos archivos que tengan una fecha de creación mayor a 1 hora
                    if (s3Object.LastModified < currentTime.AddHours(-1))
                    {
                        // 3. Borrar los archivos filtrados
                        var deleteRequest = new DeleteObjectRequest
                        {
                            BucketName = request.bucketName,
                            Key = s3Object.Key
                        };
                        await _s3Client.DeleteObjectAsync(deleteRequest);
                    }
                }
                return Unit.Value;
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
    }
}
