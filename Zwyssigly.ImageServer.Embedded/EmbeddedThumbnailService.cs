using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Contracts;

namespace Zwyssigly.ImageServer.Embedded
{
    public class EmbeddedThumbnailService : IThumbnailService
    {
        private readonly IThumbnailRepository _repository;

        public EmbeddedThumbnailService(IThumbnailRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<byte[], Contracts.Error>> GetAsync(string galleryName, string imageId, string tag)
        {
            var result = await _repository.Get(
                Name.FromString(galleryName).UnwrapOrThrow(),
                new ThumbnailId(Id.FromString(imageId).UnwrapOrThrow(), Name.FromString(tag).UnwrapOrThrow()));

            return result.Map(s => s.Data, ErrorExtensions.ToContract);
        }
    }
}
