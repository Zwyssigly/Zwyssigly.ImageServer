using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;
using Zwyssigly.ImageServer.Core;
using Zwyssigly.ImageServer.Processing;
using Zwyssigly.ImageServer.Thumbnails;

namespace Zwyssigly.ImageServer.Images
{
    public class ImageUploadService 
    {
        private readonly IConfigurationStorage _configurationStorage;
        private readonly IImageRepository _imageRepository;
        private readonly IThumbnailRepository _thumbnailRepository;
        private readonly IIdGenerator _imageIdGenerator;
        private readonly IClock _clock;
        private readonly IHttpClientWrapper _httpClient;
        private readonly IImageFactory _imageFactory;

        public ImageUploadService(
            IConfigurationStorage configurationStorage,
            IImageRepository imageRepository,
            IThumbnailRepository thumbnailRepository,
            IIdGenerator imageIdGenerator,
            IClock clock,
            IHttpClientWrapper httpClient,
            IImageFactory imageFactory)
        {
            _configurationStorage = configurationStorage;
            _imageRepository = imageRepository;
            _thumbnailRepository = thumbnailRepository;
            _imageIdGenerator = imageIdGenerator;
            _clock = clock;
            _httpClient = httpClient;
            _imageFactory = imageFactory;
        }

        public Task<Result<Unit, Error>> Delete(Name galleryName, IEnumerable<Id> ids)
        {
            return _imageRepository
                .Get(galleryName, ids)
                .AndThenAsync(images =>
                {
                    return _imageRepository
                        .Delete(galleryName, images.Select(i => i.Id))
                        .AndThenAsync(_ => _thumbnailRepository.Delete(galleryName, images.SelectMany(i => i.ThumbnailIds)));
                });
        }

        public Task<Result<Image, Error>> ReplaceAsync(Name galleryName, Id imageId, byte[] data, Option<byte[]> meta)
        {
            return _imageRepository
                .Get(galleryName, new[] { imageId })
                .AndThenAsync(oldImage => CreateImageWithThumbnails(galleryName, oldImage.Single().Id, oldImage.Single().RowVersion + 1u, meta, data)
                .AndThenAsync(async imageAndThumbnails =>
                {
                    var result = await _thumbnailRepository
                        .Insert(galleryName, imageAndThumbnails.Thumbnails)
                        .AndThenAsync(_ => _imageRepository.Update(galleryName, new[] { imageAndThumbnails.Image }))
                        .AndThenAsync(_ => _thumbnailRepository.Delete(galleryName, oldImage.Single().ThumbnailIds.Except(imageAndThumbnails.Image.ThumbnailIds)));
                    return result.MapSuccess(_ => imageAndThumbnails.Image);
                }));
        }

        public Task<Result<Image, Error>> ReplaceAsync(Name galleryName, Id imageId, string url, Option<byte[]> meta)
        {
            return _httpClient.Get(url).AndThenAsync(data => ReplaceAsync(galleryName, imageId, data, meta));
        }

        public async Task<Result<Image, Error>> UploadAsync(Name galleryName, byte[] data, Option<byte[]> meta)
        {            
            var imageId = await _imageIdGenerator.Generate().ConfigureAwait(false);
            return await CreateImageWithThumbnails(galleryName, imageId, 1u, meta, data)
                .AndThenAsync(async imageAndThumbnails =>
                {
                    var result = await _thumbnailRepository.Insert(galleryName, imageAndThumbnails.Thumbnails)
                        .AndThenAsync(_ => _imageRepository.Insert(galleryName, new[] { imageAndThumbnails.Image }));
                    return result.MapSuccess(_ => imageAndThumbnails.Image);
                });
        }

        public Task<Result<Image, Error>> UploadAsync(Name galleryName, string url, Option<byte[]> meta)
        {
            return _httpClient.Get(url).AndThenAsync(data => UploadAsync(galleryName, data, meta));
        }

        public Task<Result<(Image Image, IReadOnlyCollection<Thumbnail> Thumbnails), Error>> CreateImageWithThumbnails(
            Name galleryName, Id id, uint rowVersion, Option<byte[]> meta, byte[] raw)
        {
            return _configurationStorage
                .Get(galleryName)
                .AndThenAsync(configuration => _imageFactory.Create(id, rowVersion, meta, raw, configuration));
        }
    }
}
