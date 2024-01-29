using Microsoft.AspNetCore.Http;

namespace Microsoft.MecSolutionAccelerator.Services.Files.Infraestructure
{
    public interface IFileStorage
    {
        public Task<MemoryStream> DownloadFile(string fileName, string containerName, CancellationToken cancellationToken);
        public Task DeleteOlderThanFiles(string containerName, int hours, CancellationToken cancellationToken);
        public Task<Guid> Handle(IFormFile FormFile, string containerName, string sourceId, long timestamp, CancellationToken cancellationToken);
    }
}
