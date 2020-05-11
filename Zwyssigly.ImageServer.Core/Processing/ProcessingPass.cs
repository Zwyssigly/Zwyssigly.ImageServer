using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Processing
{
    public class ImageProcessingPass : SimpleValueObject<ImageProcessingPass>
    {
        public Resolution Resolution { get; }
        public Option<CropStrategy> Crop { get; }
        public ImageFormat ImageFormat { get; }
        public Quality Quality { get; }
        public Color Color { get; }

        public ImageProcessingPass(Resolution resolution, Option<CropStrategy> crop, ImageFormat imageFormat, Quality quality, Color color)
        {
            Resolution = resolution;
            Crop = crop;
            ImageFormat = imageFormat;
            Quality = quality;
            Color = color;
        }

        public override string ToString() => $"{Color}, {Resolution}, {ImageFormat}, {Quality}" + Crop.Map(c => ", " + c).UnwrapOrDefault();

        protected override object GetEqualityFields() => new { Resolution, Crop, ImageFormat, Quality, Color };
    }
}
