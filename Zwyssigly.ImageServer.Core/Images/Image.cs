using NodaTime;
using System.Collections.Generic;
using System.Linq;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer
{

    public class Image
    {
        public Id Id { get; }

        public uint RowVersion { get; }

        public Instant UploadedAt { get; }
        public Option<byte[]> Meta { get; }

        public Color FillColor { get; }
        public Color EdgeColor { get; }

        public Md5 Md5 { get; }

        public IReadOnlyCollection<ImageSize> Sizes { get; }

        public IReadOnlyCollection<ThumbnailId> ThumbnailIds => Sizes.Select(s => new ThumbnailId(Id, s.Tag)).ToArray();

        public static Result<Image, Error> New(Id id, uint rowVersion, Instant uploadedAt, Option<byte[]> meta, Color fillColor, Color edgeColor, Md5 md5, IReadOnlyCollection<ImageSize> sizes)
        {
            if (rowVersion < 1u)
                return Result.Failure(Error.ValidationError("Row version must be greater than 0"));

            if (sizes.Select(s => s.Tag).Distinct().Count() < sizes.Count)
                return Result.Failure(Error.ValidationError("Sizes must have unique tags"));

            return Result.Success(new Image(id, rowVersion, uploadedAt, meta, fillColor, edgeColor, md5, sizes));
        }

        private Image(Id id, uint rowVersion, Instant uploadedAt, Option<byte[]> meta, Color fillColor, Color edgeColor, Md5 md5, IReadOnlyCollection<ImageSize> sizes)
        {
            Id = id;
            RowVersion = rowVersion;
            UploadedAt = uploadedAt;
            Meta = meta;
            FillColor = fillColor;
            EdgeColor = edgeColor;
            Md5 = md5;
            Sizes = sizes;
        }
    }
}
