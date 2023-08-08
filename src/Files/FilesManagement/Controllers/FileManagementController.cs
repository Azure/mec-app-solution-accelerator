using FilesManagement.CommandHandler;
using FilesManagement.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FilesManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FileManagementController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                var fileId = await _mediator.Send(new UploadNewFileCommand() { FormFile = file, BucketName = "images" });
                return Ok(new { Id = fileId });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> DownloadImage(Guid fileId)
        {
            try
            {
                var fileName = $"{fileId}.jpg"; // File extension
                var bucketName = "images";
                var response = await _mediator.Send(new DownloadFileCommand() { FileName = fileName, BucketName = bucketName });

                return File(response.Stream, "image/jpeg", fileName); // Content type
            }
            catch (Exception ex)
            {
                // Log the error and provide an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}