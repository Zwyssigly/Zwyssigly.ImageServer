using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer
{
    public class ImageSize
    {
        public Name Tag { get; }
        public Resolution Resolution { get; }
        public AspectRatio AspectRatio { get; }
        public Option<CropStrategy> CropStrategy { get; }
        public ImageFormat ImageFormat { get; }
        public Quality Quality { get; }
        public Option<Name> DuplicateOf { get; }

        public ImageSize(Name tag, Resolution resolution, AspectRatio aspectRatio, Option<CropStrategy> cropStrategy, ImageFormat imageFormat, Quality quality, Option<Name> duplicateOf)
        {
            Tag = tag;
            Resolution = resolution;
            AspectRatio = aspectRatio;
            CropStrategy = cropStrategy;
            ImageFormat = imageFormat;
            Quality = quality;
            DuplicateOf = duplicateOf;
        }
    }
}
