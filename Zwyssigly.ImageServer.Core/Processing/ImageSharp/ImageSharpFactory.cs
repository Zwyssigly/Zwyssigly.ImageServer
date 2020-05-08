using NodaTime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    class ImageSharpFactory : IImageFactory
    {
        private readonly IClock _clock;

        private static readonly ICropStrategy[] _cropStrategies = 
         {
            new StretchStrategy(),
            new CoverStrategy(),
            new ContainStrategy()
        };

        public ImageSharpFactory(IClock clock)
        {
            _clock = clock;
        }

        public Task<Result<(Image Image, IReadOnlyCollection<Thumbnail> Thumbnails), Error>>
            Create(Id id, uint rowVersion, Option<byte[]> meta, byte[] raw, ProcessingConfiguration configuration)
        {
            Image<Rgba32>? firstImage = SixLabors.ImageSharp.Image.Load<Rgba32>(raw);

            var originalResolution = Resolution.FromScalar((ushort)firstImage.Width, (ushort)firstImage.Height).UnwrapOrThrow();
            var originalAspectRatio = AspectRatio.FromResolution(originalResolution);
            var originalColorScheme = GetColorScheme(firstImage);

            var processedSizes = new HashSet<Resolution>();
            var imageSizes = new List<ImageSize>();
            var thumbnails = new List<Thumbnail>();
            foreach (var size in configuration.Sizes)
            {
                var resolution = CalculateSize(size, originalResolution);

                if (!processedSizes.Add(resolution)) continue;

                using (var image = firstImage ?? SixLabors.ImageSharp.Image.Load<Rgba32>(raw))
                {
                    var (imageSize, thumbnail) = CreateThumbnail(
                        id,
                        size,
                        resolution,
                        originalAspectRatio,
                        originalColorScheme,
                        image
                    );

                    imageSizes.Add(imageSize);
                    thumbnails.Add(thumbnail);
                }

                firstImage = null;
            }

            return Task.FromResult(
                Image.New(
                    id,
                    rowVersion,
                    _clock.GetCurrentInstant(),
                    meta,
                    originalColorScheme.FillColor,
                    originalColorScheme.EdgeColor,
                    Md5.ComputeHash(raw).UnwrapOrThrow(),
                    imageSizes
                )
                .MapSuccess<(Image Image, IReadOnlyCollection<Thumbnail> Thumbnails)>(image => (image, thumbnails))
            );
        }

        public static Resolution CalculateSize(SizeConfiguration size, Resolution resolution)
        {
            size.Crop.IfSome(crop =>
            {
                var strategy = _cropStrategies.First(s => s.CanCrop(crop));
                resolution = strategy.CalculateResolution(resolution, crop);
            });

            return resolution.Max(size.MaxWidth, size.MaxHeight);
        }

        private ColorScheme GetColorScheme(Image<Rgba32> image)
        {
            Color FromVector4(IReadOnlyCollection<Vector4> values)
            {
                var value = values.Aggregate(Vector4.Zero, (a, b) => a + b) / values.Count;
                return Color.FromString(((SixLabors.ImageSharp.Color)value).ToHex().Substring(0, 6)).UnwrapOrThrow();
            }

            var fillColors = new Vector4[image.Height];
            var edgeColors = new List<Vector4>(image.Width * 2 + image.Height * 2 - 4);

            for (int y = 0; y < image.Height; y++)
            {
                var line = Enumerable.Range(0, image.Width).Select(x => image[x, y].ToScaledVector4()).ToArray();
                fillColors[y] = line.Aggregate(Vector4.Zero, (a, b) => a + b) / line.Length;
                if (y > 0 || y < image.Height - 1)
                {
                    edgeColors.Add(image[0, y].ToScaledVector4());
                    edgeColors.Add(image[image.Width-1, y].ToScaledVector4());
                }
                else
                    edgeColors.AddRange(line);
            }

            return new ColorScheme(
                fillColor: FromVector4(fillColors),
                edgeColor: FromVector4(edgeColors)
            );
        }

        private (ImageSize, Thumbnail) CreateThumbnail(
            Id imageId,
            SizeConfiguration configuration, 
            Resolution resolution,
            AspectRatio original,
            ColorScheme colorScheme,
            SixLabors.ImageSharp.Image image)
        {
            Option<CropStrategy> cropStrategy = default;

            configuration.Crop.Match(
                crop =>
                {
                    if (crop.AspectRatio == original)
                    {
                        image.Mutate(ctx => ctx.Resize(resolution.Width, resolution.Height));
                        return;
                    }

                    var strategy = _cropStrategies.First(c => c.CanCrop(crop));
                    strategy.CropImage(image, resolution, crop, colorScheme);

                    cropStrategy = Option.Some(crop.CropStrategy);
                },
                () => image.Mutate(ctx => ctx.Resize(resolution.Width, resolution.Height))
            );

            using var stream = new MemoryStream();
            if (configuration.Format == ImageFormat.Jpeg)
                image.SaveAsJpeg(stream, new JpegEncoder { Quality = (int)(configuration.Quality.ToScaler() * 100) });
            else if (configuration.Format == ImageFormat.Png)
                image.SaveAsPng(stream, new PngEncoder { CompressionLevel = (int)(configuration.Quality.ToScaler() * 8) + 1 });
            else
                throw new InvalidOperationException();

            var thumbnail = new Thumbnail(
                thumbnailId: new ThumbnailId(imageId, configuration.Tag),
                data: stream.ToArray()
            );

            var meta = new ImageSize(
                tag: configuration.Tag,
                resolution: resolution,
                aspectRatio: configuration.Crop.Map(c => c.AspectRatio).UnwrapOr(() => original),
                cropStrategy: cropStrategy,
                imageFormat: configuration.Format
           );

            return (meta, thumbnail);
        }
    }
}
