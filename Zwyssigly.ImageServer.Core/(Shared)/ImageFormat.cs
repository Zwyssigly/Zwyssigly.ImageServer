using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public sealed class ImageFormat : SimpleValueObject<ImageFormat>
    {
        public static readonly ImageFormat Jpeg = new ImageFormat("jpg", "image/jpeg", new ValueObjectSet<string>("jpeg", "jpg"));
        public static readonly ImageFormat Png = new ImageFormat("png", "image/png", new ValueObjectSet<string>("png"));

        public static Result<ImageFormat, Error> FromExtension(string ext)
        {
            ext = ext.ToLower();

            if (Jpeg.FileExtensions.Contains(ext))
                return Result.Success(Jpeg);

            if (Png.FileExtensions.Contains(ext))
                return Result.Success(Png);

            return Result.Failure(Error.ValidationError($"Unkown file extension '{ext}'"));
        }

        public static Result<ImageFormat, string> FromMimeType(string mimeType)
        {
            mimeType = mimeType.ToLower();

            if (Jpeg.MimeType == mimeType)
                return Result.Success(Jpeg);

            if (Png.MimeType == mimeType)
                return Result.Success(Png);

            return Result.Failure($"Unkown mime type '{mimeType}'");
        }

        public string FileExtension { get; }
        public string MimeType { get; }
        public ValueObjectSet<string> FileExtensions { get; }

        private ImageFormat(string fileExtension, string mimeType, ValueObjectSet<string> fileExtensions)
        {
            FileExtension = fileExtension;
            MimeType = mimeType;
            FileExtensions = fileExtensions;
        }

        protected override object GetEqualityFields() => FileExtension;

        public override string ToString() => FileExtension;
    }
}
