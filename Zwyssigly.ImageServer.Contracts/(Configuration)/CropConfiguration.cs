namespace Zwyssigly.ImageServer.Contracts
{
    public class CropConfiguration
    {
        public string AspectRatio { get; }
        public string CropStrategy { get; }
        public string? Color { get; }

        public CropConfiguration(string aspectRatio, string cropStrategy, string? color)
        {
            AspectRatio = aspectRatio;
            CropStrategy = cropStrategy;
            Color = color;
        }
    }
}
