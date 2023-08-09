using MediatR;

namespace FilesManagement.Commands
{
    public class UploadNewFileCommand : IRequest<Guid>
    {
        public IFormFile FormFile { get; set; }
        public string BucketName { get; set; }
        public string SourceId { get; set; }
        public long Timestamp { get; set; }
    }
}
