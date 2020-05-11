using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Images;
using Zwyssigly.ImageServer.Security;
using Zwyssigly.ImageServer.Thumbnails;

namespace Zwyssigly.ImageServer.Embedded
{
    class EmbeddedGalleryService : IGalleryService
    {
        private readonly IConfigurationStorage _storage;
        private readonly IImageRepository _imageRepository;
        private readonly IThumbnailRepository _thumbnailRepository;
        private readonly ISecurityConfigurationStorage _securityStorage;

        public EmbeddedGalleryService(
            IConfigurationStorage storage,
            IImageRepository imageRepository,
            IThumbnailRepository thumbnailRepository,
            ISecurityConfigurationStorage securityStorage)
        {
            _storage = storage;
            _imageRepository = imageRepository;
            _thumbnailRepository = thumbnailRepository;
            _securityStorage = securityStorage;
        }

        public async Task<Result<Unit, Contracts.Error>> DeleteAsync(string galleryName)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => _imageRepository.Truncate(name)
                .AndThenAsync(_ => _thumbnailRepository.Truncate(name))
                .AndThenAsync(_ => _storage.Delete(name))
                .AndThenAsync(_ => _securityStorage.Delete(name)));

            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<IReadOnlyCollection<string>, Contracts.Error>> ListAsync()
        {
            var result = await _storage.List();
            return result.Map(
                s => (IReadOnlyCollection<string>)s.Select(g => g.ToString()).ToArray(),
                ErrorExtensions.ToContract);
        }

        public async Task<Result<Unit, Contracts.Error>> NewAsync(string galleryName)
        {

            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => _storage.Persist(name, Configuration.ProcessingConfiguration.Default)
                .AndThenAsync(_ => _securityStorage.Persist(name, Security.SecurityConfiguration.Default)));

            return result.MapFailure(ErrorExtensions.ToContract);
        }
    }
}
