using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Images;

namespace Zwyssigly.ImageServer.Thumbnails
{
    public class ThumbnailResolver
    {
        private readonly IImageRepository _repository;

        public ThumbnailResolver(IImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IReadOnlyList<ResolvedThumbnail>, Error>> Resolve(Name galleryName, IEnumerable<Id> ids, ResolveOptions options)
        {
            if (options.Tag.IsNone && options.MinWidth.IsNone && options.MinHeight.IsNone)
                return Result.Failure(Error.ValidationError("At least one resolve option is required"));

            var result = await _repository.Get(galleryName, ids);
            return result.AndThen<IReadOnlyList<ResolvedThumbnail>>(ts => ts.Select(t => Resolve(t, options)).Railway());
        }

        private Result<ResolvedThumbnail, Error> Resolve(Image image, ResolveOptions options)
        {
            return options.Tag.AndThen(t => ResolveByTag(image, t))
                .OrThen(() => options.MinWidth.AndThen(mw => options.MinHeight.AndThen(mh => ResolveBySize(image, mw, mh))))
                .OrThen(() => options.MinWidth.AndThen(mw => ResolveByWidth(image, mw)))
                .OrThen(() => options.MinHeight.AndThen(mh => ResolveByHeight(image, mh)))
                .OrThen(() => ResolveDuplicate(image, image.Sizes.First()))
                .Match(
                    s => Result.Success<ResolvedThumbnail, Error>(s),
                    () => Result.Failure<ResolvedThumbnail, Error>(ErrorCode.NoSuchRecord)
                );
        }

        private Option<ResolvedThumbnail> ResolveByTag(Image image, Name t)
        {
            return image.Sizes.FirstOrNone(s => s.Tag == t).AndThen(s => ResolveDuplicate(image, s));
        }

        private Option<ResolvedThumbnail> ResolveDuplicate(Image image, ImageSize s)
        {
            return s.DuplicateOf.Match(t => ResolveByTag(image, t), () => Option.Some(new ResolvedThumbnail(
                id: image.Id,
                rowVersion: image.RowVersion,
                fillColor: image.FillColor,
                edgeColor: image.EdgeColor,
                tag: s.Tag,
                format: s.ImageFormat,
                resolution: s.Resolution
            )));
        }

        private Option<ResolvedThumbnail> ResolveBySize(Image image, ushort width, ushort height)
        {
            var ordered = image.Sizes.OrderBy(s => s.Resolution.Height * s.Resolution.Width).ToArray();
            var size = ordered
                .FirstOrNone(i => i.Resolution.Width >= width && i.Resolution.Height > height)
                .UnwrapOr(() => ordered.Last());

            return ResolveDuplicate(image, size);
        }

        private Option<ResolvedThumbnail> ResolveByWidth(Image image, ushort width)
        {
            var ordered = image.Sizes.OrderBy(s => s.Resolution.Width);
            var size = ordered
                .FirstOrNone(i => i.Resolution.Width >= width)
                .UnwrapOr(() => ordered.Last());

            return ResolveDuplicate(image, size);
        }

        private Option<ResolvedThumbnail> ResolveByHeight(Image image, ushort height)
        {
            var ordered = image.Sizes.OrderBy(s => s.Resolution.Height);
            var size = ordered
                .FirstOrNone(i => i.Resolution.Height >= height)
                .UnwrapOr(() => ordered.Last());

            return ResolveDuplicate(image, size);
        }
    }

    public class ResolveOptions
    {
        public Option<Name> Tag { get; set; }
        public Option<ushort> MinWidth { get; set; }
        public Option<ushort> MinHeight { get; set; }
    }
}
