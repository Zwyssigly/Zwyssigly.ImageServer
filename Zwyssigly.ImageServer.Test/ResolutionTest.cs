using NUnit.Framework;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer
{
    public class ResolutionTest
    {
        [TestCase("100x100", "4:3", ExpectedResult = "100x75")]
        [TestCase("90x30", "4:3", ExpectedResult = "40x30")]
        [TestCase("40x30", "4:3", ExpectedResult = "40x30")]
        [TestCase("100x100", "3:4", ExpectedResult = "75x100")]
        [TestCase("30x90", "3:4", ExpectedResult = "30x40")]
        [TestCase("30x40", "3:4", ExpectedResult = "30x40")]
        public string DownscaleToAspectRatioWorks(string resolution, string aspectRatio)
        {
            return Resolution
                .FromString(resolution)
                .UnwrapOrThrow()
                .Downscale(AspectRatio.FromString(aspectRatio).UnwrapOrThrow())
                .ToString();
        }

        [TestCase("30x30", "4:3", ExpectedResult = "40x30")]
        [TestCase("80x30", "4:3", ExpectedResult = "80x60")]
        [TestCase("40x30", "4:3", ExpectedResult = "40x30")]
        [TestCase("30x30", "3:4", ExpectedResult = "30x40")]
        [TestCase("30x80", "3:4", ExpectedResult = "60x80")]
        [TestCase("30x40", "3:4", ExpectedResult = "30x40")]
        public string UpscaleToAspectRatioWorks(string resolution, string aspectRatio)
        {
            return Resolution
                .FromString(resolution)
                .UnwrapOrThrow()
                .Upscale(AspectRatio.FromString(aspectRatio).UnwrapOrThrow())
                .ToString();
        }

        [TestCase("100x100", null, null, ExpectedResult = "100x100")]
        [TestCase("100x100", 100u, 100u, ExpectedResult = "100x100")]
        [TestCase("100x100", 1000u, 1000u, ExpectedResult = "100x100")]
        [TestCase("100x100", 1000u, null, ExpectedResult = "100x100")]
        [TestCase("100x100", null, 1000u, ExpectedResult = "100x100")]
        [TestCase("100x100", 40u, 30u, ExpectedResult = "30x30")]
        [TestCase("100x100", 30u, 40u, ExpectedResult = "30x30")]
        [TestCase("1920x1920", 500u, 500u, ExpectedResult = "500x500")]
        [TestCase("1200x1200", 500u, 500u, ExpectedResult = "500x500")]
        public string MaxWorks(string resolution, uint? maxWidth, uint? maxHeight)
        {
            return Resolution
                .FromString(resolution)
                .UnwrapOrThrow()
                .Max(maxWidth.ToOption().Map(u => (ushort) u), maxHeight.ToOption().Map(u => (ushort)u))
                .ToString();
        }
    }
}
