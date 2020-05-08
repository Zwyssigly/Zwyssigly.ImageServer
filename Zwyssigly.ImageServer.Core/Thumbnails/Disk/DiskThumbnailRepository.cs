using System.Collections.Generic;
using System.IO;
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
            return Path.Combine(_options.Directory, gallery.ToString(), id.ToString());
        }

        private string GeneratePath(Name gallery)
        {
            return Path.Combine(_options.Directory, gallery.ToString());
        }

        public Task<Result<Unit, Error>> Delete(Name gallery, IEnumerable<ThumbnailId> ids)
        {
            foreach (var id in ids)
                File.Delete(GeneratePath(gallery, id));

            return Task.FromResult(Result.Unit<Error>());
        }

        public Task<Result<Thumbnail, Error>> Get(Name gallery, ThumbnailId id)
        {
            var data = File.ReadAllBytes(GeneratePath(gallery, id));
            return Task.FromResult(Result.Success<Thumbnail, Error>(new Thumbnail(id, data)));
        }

        public Task<Result<Unit, Error>> Insert(Name gallery, IEnumerable<Thumbnail> thumbnails)
        {
            Directory.CreateDirectory(GeneratePath(gallery));

            foreach (var thumbnail in thumbnails)
                File.WriteAllBytes(GeneratePath(gallery, thumbnail.ThumbnailId), thumbnail.Data);

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
