using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Configuration
{
    public class SizeConfiguration
    {
        public Name Tag { get; }
        public Option<ushort> MaxWidth { get; }
        public Option<ushort> MaxHeight { get; }
        public Option<CropConfiguration> Crop { get; }
        public Quality Quality { get; }
        public ImageFormat Format { get; }

        public SizeConfiguration(
            Name tag, 
            Option<ushort> maxWidth, 
            Option<ushort> maxHeight, 
            Option<CropConfiguration> crop, 
            Quality quality, 
            ImageFormat format)
        {
            Tag = tag;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Crop = crop;
            Quality = quality;
            Format = format;
        }
    }
}
