namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    class ImageSharpProcessorFactory : IImageProcessorFactory
    {
        public IImageProcessor Create(byte[] data) => new ImageSharpProcessor(data);
    }
}
