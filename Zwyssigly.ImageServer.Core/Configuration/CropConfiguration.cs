using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Configuration
{
    public class CropConfiguration : SimpleValueObject<CropConfiguration>
    {
        public AspectRatio AspectRatio { get; }
        public CropStrategy CropStrategy { get; }
        public Option<Color> BackgroundColor { get; }

        public CropConfiguration(
            AspectRatio aspectRatio, 
            CropStrategy cropStrategy, 
            Option<Color> backgroundColor)
        {
            AspectRatio = aspectRatio;
            CropStrategy = cropStrategy;
            BackgroundColor = backgroundColor;
        }

        protected override object GetEqualityFields() => new { AspectRatio, CropStrategy, BackgroundColor };

        public override string ToString() => $"{AspectRatio}, {CropStrategy}, {BackgroundColor}";
    }
}
