using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Files.Commands;
using Microsoft.MecSolutionAccelerator.Services.Files.Infraestructure;

namespace Microsoft.MecSolutionAccelerator.Services.Files.CommandHandlers
{
    public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, DownloadFileResponse>
    {
        private readonly IFileStorage _filesService;
        private readonly ILogger<DownloadFileCommandHandler> _logger;

        public DownloadFileCommandHandler(IFileStorage filesService, ILogger<DownloadFileCommandHandler> logger)
        {
            _filesService = filesService ?? throw new ArgumentNullException(nameof(filesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DownloadFileResponse> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            var responseStream = await _filesService.DownloadFile(request.FileName, request.ContainerName, cancellationToken);
            _logger.LogInformation($"Correctly downloaded Image {request.FileName}");

            return new DownloadFileResponse { Stream = responseStream };
        }
    }
}
