using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public interface IThumbnailService
    {
        Task<Result<byte[], Error>> GetAsync(string galleryName, string imageId, string tag, string? formatHint);

        Task<Result<IReadOnlyCollection<ResolvedThumbnail>, Error>> ResolveAsync(string galleryName, IEnumerable<string> ids, ResolveOptions options);
    }
}
