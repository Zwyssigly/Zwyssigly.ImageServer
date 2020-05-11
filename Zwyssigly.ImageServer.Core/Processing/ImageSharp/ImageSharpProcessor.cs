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

namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    class ImageSharpProcessor : IImageProcessor
    {
        private readonly Image<Rgba32> _image;

        public ImageSharpProcessor(byte[] raw)
        {
            _image = SixLabors.ImageSharp.Image.Load<Rgba32>(raw);
        }

        public void Dispose() =>  _image?.Dispose();

        public byte[] Process(ImageProcessingPass pass)
        {
            var options = new ResizeOptions
            {
                Size = new SixLabors.Primitives.Size(pass.Resolution.Width, pass.Resolution.Height),
                Sampler = KnownResamplers.Lanczos2
            };

            options.Mode = pass.Crop.Match(c => c switch
            {
                CropStrategy.Contain => ResizeMode.Pad,
                CropStrategy.Cover => ResizeMode.Crop,
                _ => ResizeMode.Stretch
            },
            () => ResizeMode.Stretch);

            using var image = _image.Clone(ctx => ctx
                .Resize(options)
                .BackgroundColor(SixLabors.ImageSharp.Color.FromHex(pass.Color.ToString()))
            );

            using var stream = new MemoryStream();
            if (pass.ImageFormat == ImageFormat.Jpeg)
                image.SaveAsJpeg(stream, new JpegEncoder { Quality = (int)(pass.Quality.ToScaler() * 100) });
            else if (pass.ImageFormat == ImageFormat.Png)
                image.SaveAsPng(stream, new PngEncoder { CompressionLevel = (int)(pass.Quality.ToScaler() * 8) + 1 });
            else
                throw new InvalidOperationException();

            return stream.ToArray();
        }

        public ColorScheme ReadColorScheme()
        {
            static Color FromVector4(IReadOnlyCollection<Vector4> values)
            {
                var value = values.Aggregate(Vector4.Zero, (a, b) => a + b) / values.Count;
                return Color.FromString(((SixLabors.ImageSharp.Color)value).ToHex().Substring(0, 6)).UnwrapOrThrow();
            }

            var fillColors = new Vector4[_image.Height];
            var edgeColors = new List<Vector4>(_image.Width * 2 + _image.Height * 2 - 4);

            for (int y = 0; y < _image.Height; y++)
            {
                var line = Enumerable.Range(0, _image.Width).Select(x => _image[x, y].ToScaledVector4()).ToArray();
                fillColors[y] = line.Aggregate(Vector4.Zero, (a, b) => a + b) / line.Length;
                if (y > 0 || y < _image.Height - 1)
                {
                    edgeColors.Add(_image[0, y].ToScaledVector4());
                    edgeColors.Add(_image[_image.Width - 1, y].ToScaledVector4());
                }
                else
                    edgeColors.AddRange(line);
            }

            return new ColorScheme(
                fillColor: FromVector4(fillColors),
                edgeColor: FromVector4(edgeColors)
            );
        }

        public Resolution ReadResolution()
            => Resolution.FromScalar((ushort)_image.Width, (ushort)_image.Height).UnwrapOrThrow();
    }
}
