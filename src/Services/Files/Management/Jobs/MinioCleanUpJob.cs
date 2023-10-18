using Coravel.Invocable;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Files.Commands;

namespace Microsoft.MecSolutionAccelerator.Services.FilesManagement.Jobs
{
    public class MinioCleanUpJob : IInvocable
    {
        private readonly ILogger<MinioCleanUpJob> _logger;
        private readonly IMediator _mediator;
        private readonly string CONTAINER_NAME = "images";

        public MinioCleanUpJob(ILogger<MinioCleanUpJob> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Invoke()
        {
            _logger.LogInformation("Images storage Cleaning executing...");

            try
            {
                await _mediator.Send(new DeleteFilesCommand() { containerName = CONTAINER_NAME });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the Images storage Cleaning executing....");
            }

            _logger.LogInformation("Images storage Cleaning executing....");
        }
    }
}
