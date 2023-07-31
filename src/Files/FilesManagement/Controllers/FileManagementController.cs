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
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                await _mediator.Send(new UploadNewFileCommand() {FormFile = file, BucketName = "images" });

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}