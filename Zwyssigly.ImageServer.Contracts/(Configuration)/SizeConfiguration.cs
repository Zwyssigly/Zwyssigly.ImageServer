using System;

namespace Zwyssigly.ImageServer.Contracts
{
    public class SizeConfiguration
    {
        public string Tag { get; }
        public ushort? MaxWidth { get; }
        public ushort? MaxHeight { get; }
        public CropConfiguration? Crop { get; }
        public string Format { get; }
        public float Quality { get; }

        public SizeConfiguration(string tag, ushort? maxWidth, ushort? maxHeight, CropConfiguration? crop, string format, float quality)
        {
            Tag = tag;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Crop = crop;
            Format = format;
            Quality = quality;
        }
    }
}
