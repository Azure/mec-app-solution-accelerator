using FilesManagement.CommandHandler;
using FilesManagement.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.IO;

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
        public async Task<IActionResult> UploadImage([FromBody] ImageUploadRequest request)
        {
            try
            {
                var bytes = Convert.FromBase64String(request.Image);
                var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, null, "image.jpg")
                {
                    Headers = new HeaderDictionary
                    {
                        { "X-Timestamp", request.Timestamp.ToString() }
                    },
                    ContentType = "image/jpeg"
                };

                var fileId = await _mediator.Send(new UploadNewFileCommand() { FormFile = file, BucketName = "images", SourceId = request.SourceId, Timestamp = request.Timestamp });
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

    public class ImageUploadRequest
    {
        public string SourceId { get; set; }
        public long Timestamp { get; set; }
        public string Image { get; set; } // Base64 encoded image data
    }
}