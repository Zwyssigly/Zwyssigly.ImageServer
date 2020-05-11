using NodaTime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing
{
    public class ImageFactory : IImageFactory
    {
        private readonly IClock _clock;
        private readonly IImageProcessorFactory _processorFactory;

        public ImageFactory(IClock clock, IImageProcessorFactory processorFactory)
        {
            _clock = clock;
            _processorFactory = processorFactory;
        }

        public Task<Result<(Image Image, IReadOnlyCollection<Thumbnail> Thumbnails), Error>>
           Create(Id id, uint rowVersion, Option<byte[]> meta, byte[] raw, ProcessingConfiguration configuration)
        {
            using var processor = _processorFactory.Create(raw);

            var originalResolution = processor.ReadResolution();
            var originalAspectRatio = AspectRatio.FromResolution(originalResolution);
            var originalColorScheme = processor.ReadColorScheme();

            var passByTag = new Dictionary<ImageProcessingPass, Name>();
            var imageSizes = new List<ImageSize>();
            var thumbnails = new List<Thumbnail>();

            foreach (var size in configuration.Sizes)
            {
                var resolution = CalculateSize(size, originalResolution);

                var crop = size.Crop.Match(c => c.AspectRatio != originalAspectRatio ? Option.Some(c.CropStrategy) : Option.None(), () => Option.None());

                var color = size.Crop.Match(c => c.BackgroundColor, () => Option.None()).UnwrapOr(() => originalColorScheme.EdgeColor);

                var pass = new ImageProcessingPass(resolution, crop, size.Format, size.Quality, color);

                if (configuration.AvoidDuplicates)
                {
                    if (passByTag.TryGetValue(pass, out var duplicate))
                    {
                        imageSizes.Add(new ImageSize(
                            size.Tag,
                            resolution,
                            size.Crop.Match(c => c.AspectRatio, () => originalAspectRatio),
                            crop,
                            size.Format,
                            size.Quality,
                            Option.Some(duplicate)));

                        continue;
                    }
                }

                passByTag[pass] = size.Tag;

                var data = processor.Process(pass);

                thumbnails.Add(new Thumbnail(
                    thumbnailId: new ThumbnailId(id, size.Tag, Option.Some(size.Format)),
                    data: data
                ));

                imageSizes.Add(new ImageSize(
                    tag: size.Tag,
                    resolution: resolution,
                    aspectRatio: size.Crop.Match(c => c.AspectRatio, () => originalAspectRatio),
                    cropStrategy: crop,
                    imageFormat: size.Format,
                    quality: size.Quality,
                    duplicateOf: Option.None()
               ));
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
            size.Crop.IfSome(crop => resolution = crop.CropStrategy switch
            {
                CropStrategy.Contain => resolution.Upscale(crop.AspectRatio),
                CropStrategy.Cover => resolution.Downscale(crop.AspectRatio),
                CropStrategy.Stretch => resolution.Downscale(crop.AspectRatio),
                _ => throw new InvalidOperationException()
            }); 

            return resolution.Max(size.MaxWidth, size.MaxHeight);
        }
    }
}
