using SixLabors.ImageSharp.Processing;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    internal class CoverStrategy : ICropStrategy
    {
        public Resolution CalculateResolution(Resolution resolution, CropConfiguration configuration)
            => resolution.Downscale(configuration.AspectRatio);

        public bool CanCrop(CropConfiguration configuration)
            => configuration.CropStrategy == CropStrategy.Cover;

        public void CropImage(SixLabors.ImageSharp.Image image, Resolution resolution, CropConfiguration configuration, ColorScheme colorScheme)
        {
            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new SixLabors.Primitives.Size(resolution.Width, resolution.Height),
                Sampler = KnownResamplers.Lanczos2
            }));
        }
    }
}
