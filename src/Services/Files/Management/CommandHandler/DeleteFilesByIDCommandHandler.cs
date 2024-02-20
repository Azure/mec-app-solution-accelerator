using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Files.Commands;
using Microsoft.MecSolutionAccelerator.Services.Files.Infraestructure;

namespace Microsoft.MecSolutionAccelerator.Services.Files.CommandHandlers
{
    public class DeleteFilesByIDCommandHandler : IRequestHandler<DeleteFilesCommand>
    {
        private readonly IFileStorage _filesService;
        private readonly ILogger<DeleteFilesByIDCommandHandler> _logger;
        private const int MAX_FILE_TIME_ALIVE = 15;

        public DeleteFilesByIDCommandHandler(IFileStorage filesService, ILogger<DeleteFilesByIDCommandHandler> logger)
        {
            _filesService = filesService ?? throw new ArgumentNullException(nameof(filesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
        {
            await this._filesService.DeleteOlderThanFiles(request.containerName, MAX_FILE_TIME_ALIVE, cancellationToken);
            _logger.LogInformation($"Correctly, deleted files olders than {MAX_FILE_TIME_ALIVE} minutes");

            return Unit.Value;
        }
    }
}
