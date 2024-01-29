using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Files.Commands;

namespace Microsoft.MecSolutionAccelerator.Services.Files.ConfigurationControllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly string CONTAINER_NAME = "images";
        public FileManagementController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> DownloadImage(Guid fileId)
        {
            try
            {
                var fileName = $"{fileId}.jpg"; 
                var response = await _mediator.Send(new DownloadFileCommand() { FileName = fileName, ContainerName = CONTAINER_NAME });

                return File(response.Stream, "image/jpeg", fileName); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}