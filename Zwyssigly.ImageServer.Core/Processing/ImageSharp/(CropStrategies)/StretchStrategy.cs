using SixLabors.ImageSharp.Processing;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    internal class StretchStrategy : ICropStrategy
    {
        public Resolution CalculateResolution(Resolution resolution, CropConfiguration configuration)
            => resolution.Downscale(configuration.AspectRatio);

        public bool CanCrop(CropConfiguration configuration)
            => configuration.CropStrategy == CropStrategy.Stretch;

        public void CropImage(SixLabors.ImageSharp.Image image, Resolution resolution, CropConfiguration configuration, ColorScheme colorScheme)
        {
            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Stretch,
                Size = new SixLabors.Primitives.Size(resolution.Width, resolution.Height),
                Sampler = KnownResamplers.Lanczos2
            }));
        }
    }
}
