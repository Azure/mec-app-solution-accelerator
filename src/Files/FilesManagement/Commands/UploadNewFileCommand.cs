using MediatR;

namespace FilesManagement.Commands
{
    public class UploadNewFileCommand : IRequest<Guid>
    {
        public IFormFile FormFile { get; set; }
        public string BucketName { get; set; }
    }
}
