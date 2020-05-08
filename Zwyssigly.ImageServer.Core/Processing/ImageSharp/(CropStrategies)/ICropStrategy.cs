using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    internal interface ICropStrategy
    {
        bool CanCrop(CropConfiguration configuration);

        Resolution CalculateResolution(Resolution resolution, CropConfiguration configuration);

        void CropImage(SixLabors.ImageSharp.Image image, Resolution resolution, CropConfiguration configuration, ColorScheme colorScheme);
    }
}
