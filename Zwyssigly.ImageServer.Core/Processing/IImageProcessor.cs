using System;

namespace Zwyssigly.ImageServer.Processing
{
    public interface IImageProcessor : IDisposable
    {
        Resolution ReadResolution();
        ColorScheme ReadColorScheme();
        byte[] Process(ImageProcessingPass pass);
    }
}
