namespace Zwyssigly.ImageServer.Contracts
{
    public class ImageSize
    {
        public string Tag { get; }
        public string AspectRatio { get; } 
        public ushort Width { get; }
        public ushort Height { get; }
        public string? CropStrategy { get; }
        public string Format { get; }

        public ImageSize(string tag, string aspectRatio, ushort width, ushort height, string? cropStrategy, string format)
        {
            Tag = tag;
            AspectRatio = aspectRatio;
            Width = width;
            Height = height;
            CropStrategy = cropStrategy;
            Format = format;
        }
    }
}
