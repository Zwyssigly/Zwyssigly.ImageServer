using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Images;

namespace Zwyssigly.ImageServer.Embedded
{
    class EmbeddedImageService : IImageService
    {
        private readonly ImageUploadService _uploadService;
        private readonly IImageRepository _repository;

        public EmbeddedImageService(ImageUploadService uploadService, IImageRepository repository)
        {
            _uploadService = uploadService;
            _repository = repository;
        }

        public async Task<Result<Unit, Contracts.Error>> DeleteAsync(string galleryName, IEnumerable<string> ids)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => ToIds(ids)
                .AndThenAsync(i => _uploadService.Delete(name, i)));

            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<IReadOnlyCollection<Contracts.Image>, Contracts.Error>> GetAsync(string galleryName, IEnumerable<string> ids)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => ToIds(ids)
                .AndThenAsync(i => _repository.Get(name, i)));

            return result.Map(
                s => (IReadOnlyCollection<Contracts.Image>)s.Select(ToContract).ToArray(),
                ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.Page<Contracts.Image>, Contracts.Error>> ListAsync(string galleryName, uint skip, uint take)
        {
            var result = await Name.FromString(galleryName).AndThenAsync(name => _repository.List(name, skip, take));
            return result.Map(
                p => new Contracts.Page<Contracts.Image>(p.Items.Select(ToContract).ToArray(), p.TotalItems),
                ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.Image, Contracts.Error>> ReplaceAsync(string galleryName, string imageId, byte[] data, byte[] meta)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => Id.FromString(imageId)
                .AndThenAsync(id => _uploadService.ReplaceAsync(name, id, data, meta?.Length > 0 ? Option.Some(meta) : Option.None())));

            return result.Map(ToContract, ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.Image, Contracts.Error>> ReplaceAsync(string galleryName, string imageId, string url, byte[] meta)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => Id.FromString(imageId)
                .AndThenAsync(id => _uploadService.ReplaceAsync(name, id, url, meta?.Length > 0 ? Option.Some(meta) : Option.None())));

            return result.Map(ToContract, ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.Image, Contracts.Error>> UploadAsync(string galleryName, byte[] data, byte[] meta)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => _uploadService.UploadAsync(name, data, meta?.Length > 0 ? Option.Some(meta) : Option.None()));

            return result.Map(ToContract, ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.Image, Contracts.Error>> UploadAsync(string galleryName, string url, byte[] meta)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(name => _uploadService.UploadAsync(name, url, meta?.Length > 0 ? Option.Some(meta) : Option.None()));

            return result.Map(ToContract, ErrorExtensions.ToContract);
        }

        private Contracts.Image ToContract(Image image)
        {
            return new Contracts.Image(
                id: image.Id.ToString(),
                rowVersion: image.RowVersion,
                meta: image.Meta.UnwrapOrDefault(),
                uploadedAt: image.UploadedAt.ToDateTimeUtc(),
                edgeColor: image.EdgeColor.ToString(),
                fillColor: image.FillColor.ToString(),
                md5: image.Md5.ToByteArray(),
                sizes: image.Sizes.Select(s => new Contracts.ImageSize(
                    tag: s.Tag.ToString(),
                    aspectRatio: s.AspectRatio.ToString(),
                    width: s.Resolution.Width,
                    height: s.Resolution.Height,
                    cropStrategy: s.CropStrategy.Map(f => f.ToString()).UnwrapOrDefault(),
                    format: s.ImageFormat.FileExtension,
                    quality: s.Quality.ToScaler(),
                    duplicateOf: s.DuplicateOf.Map(d => d.ToString()).UnwrapOrDefault()
                )).ToArray()
            );
        }

        private Result<IReadOnlyList<Id>, Error> ToIds(IEnumerable<string> ids)
            => ids.Select(id => Id.FromString(id)).Railway();
    }
}
