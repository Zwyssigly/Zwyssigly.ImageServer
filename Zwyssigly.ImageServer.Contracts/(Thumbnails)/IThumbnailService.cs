using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public interface IThumbnailService
    {
        Task<Result<byte[], Error>> GetAsync(string galleryName, string imageId, string tag);
    }
}
