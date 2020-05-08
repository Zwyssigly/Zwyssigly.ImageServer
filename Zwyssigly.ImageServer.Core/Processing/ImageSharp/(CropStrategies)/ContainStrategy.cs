using SixLabors.ImageSharp.Processing;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    internal class ContainStrategy : ICropStrategy
    {
        public Resolution CalculateResolution(Resolution resolution, CropConfiguration configuration)
            => resolution.Upscale(configuration.AspectRatio);

        public bool CanCrop(CropConfiguration configuration)
            => configuration.CropStrategy == CropStrategy.Contain;

        public void CropImage(SixLabors.ImageSharp.Image image, Resolution resolution, CropConfiguration configuration, ColorScheme colorScheme)
        {
            var color = configuration.BackgroundColor.UnwrapOr(() => colorScheme.EdgeColor);

            image.Mutate(ctx => ctx
                .Resize(new ResizeOptions
                {
                    Size = new SixLabors.Primitives.Size(resolution.Width, resolution.Height),
                    Mode = ResizeMode.Pad,
                    Sampler = KnownResamplers.Lanczos2
                })
                .BackgroundColor(SixLabors.ImageSharp.Color.FromHex(color.ToString()))
            );
        }
    }
}
