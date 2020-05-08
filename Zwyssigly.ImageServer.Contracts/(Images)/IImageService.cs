using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public interface IImageService
    {
        Task<Result<Image, Error>> UploadAsync(string galleryName, byte[] data, byte[] meta);
        Task<Result<Image, Error>> UploadAsync(string galleryName, string url, byte[] meta);
        Task<Result<Image, Error>> ReplaceAsync(string galleryName, string imageId, byte[] data, byte[] meta);
        Task<Result<Image, Error>> ReplaceAsync(string galleryName, string imageId, string url, byte[] meta);
        Task<Result<Unit, Error>> DeleteAsync(string galleryName, IEnumerable<string> ids);
        Task<Result<Page<Image>, Error>> ListAsync(string galleryName, uint skip, uint take);
        Task<Result<IReadOnlyCollection<Image>, Error>> GetAsync(string galleryName, IEnumerable<string> ids);
    }
}
