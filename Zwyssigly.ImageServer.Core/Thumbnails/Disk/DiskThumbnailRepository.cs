using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Thumbnails.Disk
{
    internal class DiskThumbnailRepository : IThumbnailRepository
    {
        private readonly DiskOptions _options;

        public DiskThumbnailRepository(DiskOptions options)
        {
            _options = options;
        }

        private string GeneratePath(Name gallery, ThumbnailId id)
        {
            var fileName = _options.FileExtensions
                ? $"{id.Tag}.{id.FormatHint.UnwrapOrThrow().FileExtension}"
                : id.Tag.ToString();

            fileName = (_options.TagDelimiter == '/')
                ? Path.Combine(id.ImageId.ToString(), fileName)
                : id.ImageId.ToString() + _options.TagDelimiter + fileName;

            return Path.Combine(_options.Directory, gallery.ToString(), fileName);
        }

        private string GeneratePath(Name gallery)
            => Path.Combine(_options.Directory, gallery.ToString());

        public Task<Result<Unit, Error>> Delete(Name gallery, IEnumerable<ThumbnailId> ids)
        {
            if (_options.FileExtensions && ids.Any(t => t.FormatHint.IsNone))
                return Task.FromResult(Result.Failure<Unit, Error>(new Error(ErrorCode.ImplementationError, "Format hint is required!")));

            foreach (var id in ids)
                File.Delete(GeneratePath(gallery, id));

            return Task.FromResult(Result.Unit<Error>());
        }

        public Task<Result<Thumbnail, Error>> Get(Name gallery, ThumbnailId id)
        {
            if (_options.FileExtensions && id.FormatHint.IsNone)
                return Task.FromResult(Result.Failure<Thumbnail, Error>(new Error(ErrorCode.ImplementationError, "Format hint is required!")));

            var data = File.ReadAllBytes(GeneratePath(gallery, id));
            return Task.FromResult(Result.Success<Thumbnail, Error>(new Thumbnail(id, data)));
        }

        public Task<Result<Unit, Error>> Insert(Name gallery, IEnumerable<Thumbnail> thumbnails)
        {
            if (_options.FileExtensions && thumbnails.Any(t => t.ThumbnailId.FormatHint.IsNone))
                return Task.FromResult(Result.Failure<Unit, Error>(new Error(ErrorCode.ImplementationError, "Format hint is required!")));

            Directory.CreateDirectory(GeneratePath(gallery));

            foreach (var thumbnail in thumbnails)
            {
                if (_options.TagDelimiter == '/')
                    Directory.CreateDirectory(Path.Combine(_options.Directory, gallery.ToString(), thumbnail.ThumbnailId.ImageId.ToString()));

                File.WriteAllBytes(GeneratePath(gallery, thumbnail.ThumbnailId), thumbnail.Data);
            }

            return Task.FromResult(Result.Unit<Error>());
        }

        public Task<Result<Unit, Error>> Truncate(Name gallery)
        {
            var galleryDirectory = new DirectoryInfo(GeneratePath(gallery));
                    
            if (galleryDirectory.Exists)
            {
                galleryDirectory.Delete(true);
            }

            return Task.FromResult(Result.Unit<Error>());
        }
    }
}
