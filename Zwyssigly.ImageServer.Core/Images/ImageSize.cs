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

        public ImageSize(Name tag, Resolution resolution, AspectRatio aspectRatio, Option<CropStrategy> cropStrategy, ImageFormat imageFormat)
        {
            Tag = tag;
            Resolution = resolution;
            AspectRatio = aspectRatio;
            CropStrategy = cropStrategy;
            ImageFormat = imageFormat;
        }
    }
}
