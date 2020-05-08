using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Thumbnail
    {
        public ThumbnailId ThumbnailId { get; }
        public byte[] Data { get; }        

        public Thumbnail(ThumbnailId thumbnailId, byte[] data)
        {
            ThumbnailId = thumbnailId;
            Data = data;            
        }
    }

    public class ThumbnailId : SimpleValueObject<ThumbnailId>
    {
        private const char Delimiter = '$';
        public Id ImageId { get; }
        public Name Tag { get; }

        public static Result<ThumbnailId, Error> FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure(Error.ValidationError("Invalid thumbnail id"));

            var parts = value.Split(new[] { Delimiter }, 2);
            if (parts.Length != 2)
                return Result.Failure(Error.ValidationError("Invalid thumbnail id"));

            return Id.FromString(parts[0]).AndThen(id => Name.FromString(parts[1]).MapSuccess(name => new ThumbnailId(id, name)));
        }

        public ThumbnailId(Id imageId, Name tag)
        {
            ImageId = imageId;
            Tag = tag;
        }

        protected override object GetEqualityFields() => new { ImageId, Tag };

        public override string ToString() => $"{ImageId}{Delimiter}{Tag}";
    }
}
