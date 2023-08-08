using MediatR;

namespace FilesManagement.Commands
{
    public class DownloadFileCommand : IRequest<DownloadFileResponse>
    {
        public string FileName { get; set; }
        public string BucketName { get; set; }
    }

    public class DownloadFileResponse
    {
        public MemoryStream Stream { get; set; }
    }

}
