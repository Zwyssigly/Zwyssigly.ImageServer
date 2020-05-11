namespace Zwyssigly.ImageServer.Processing
{
    public interface IImageProcessorFactory
    {
        public IImageProcessor Create(byte[] data);
    }
}
