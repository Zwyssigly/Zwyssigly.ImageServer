using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Thumbnails;

namespace Zwyssigly.ImageServer.Embedded
{
    public class EmbeddedThumbnailService : IThumbnailService
    {
        private readonly IThumbnailRepository _repository;
        private readonly ThumbnailResolver _resolver;

        public EmbeddedThumbnailService(IThumbnailRepository repository, ThumbnailResolver resolver)
        {
            _repository = repository;
            _resolver = resolver;
        }

        public async Task<Result<byte[], Contracts.Error>> GetAsync(string galleryName, string imageId, string tag, string formatHint)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(gallery => Id.FromString(imageId)
                .AndThenAsync(id => Name.FromString(tag)
                .AndThenAsync(t => (formatHint != null ? ImageFormat.FromExtension(formatHint).MapSuccess(s => Option.Some(s)) : Result.Success(Option.None<ImageFormat>()))
                .AndThenAsync(hint => _repository.Get(gallery, new ThumbnailId(id, t, hint))))));
            
            return result.Map(s => s.Data, ErrorExtensions.ToContract);
        }

        public async Task<Result<IReadOnlyCollection<Contracts.ResolvedThumbnail>, Contracts.Error>> ResolveAsync(string galleryName, IEnumerable<string> ids, Contracts.ResolveOptions options)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(gallery => ToIds(ids)
                .AndThenAsync(id => FromContract(options)
                .AndThenAsync(opts => _resolver.Resolve(gallery, id, opts))));

            return result.Map<IReadOnlyCollection<Contracts.ResolvedThumbnail>, Contracts.Error>(t => t.Select(ToContract).ToArray(), ErrorExtensions.ToContract);
        }

        private Contracts.ResolvedThumbnail ToContract(Thumbnails.ResolvedThumbnail thumbnail)
        {
            return new Contracts.ResolvedThumbnail(
                id: thumbnail.Id.ToString(),
                rowVersion: thumbnail.RowVersion,
                fillColor: thumbnail.FillColor.ToString(),
                edgeColor: thumbnail.EdgeColor.ToString(),
                tag: thumbnail.Tag.ToString(),
                format: thumbnail.Format.FileExtension,
                width: thumbnail.Resolution.Width,
                height: thumbnail.Resolution.Height);
        }

        private Result<Thumbnails.ResolveOptions, Error> FromContract(Contracts.ResolveOptions options)
        {
            var tagResult = string.IsNullOrEmpty(options.Tag)
                ? Result.Success<Option<Name>, Error>(Option.None<Name>())
                : Name.FromString(options.Tag).MapSuccess(t => Option.Some(t)); 

            return tagResult.MapSuccess(t => new Thumbnails.ResolveOptions
            {
                Tag = t,
                MinWidth = options.MinWidth.ToOption(),
                MinHeight = options.MinHeight.ToOption()
            });
        }

        private Result<IReadOnlyList<Id>, Error> ToIds(IEnumerable<string> ids)
            => ids.Select(id => Id.FromString(id)).Railway();
    }
}
