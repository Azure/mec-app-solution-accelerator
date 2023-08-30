using MediatR;

namespace Microsoft.MecSolutionAccelerator.Services.Files.Commands
{
    public class UploadNewFileCommand : IRequest<Guid>
    {
        public IFormFile FormFile { get; set; }
        public string BucketName { get; set; }
        public string SourceId { get; set; }
        public long Timestamp { get; set; }
    }
}
