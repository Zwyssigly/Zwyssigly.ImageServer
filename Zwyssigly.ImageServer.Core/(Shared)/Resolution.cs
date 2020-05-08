using System;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Resolution : SimpleValueObject<Resolution>
    {
        public ushort Width { get; }
        public ushort Height { get; }

        public static Result<Resolution, string> FromString(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return Result.Failure("Can not create aspect ratio from empty string");

            var parts = raw.Split('x');
            if (parts.Length != 2)
                return Result.Failure("Format 1920x1080 expected");

            if (!int.TryParse(parts[0], out var w))
                return Result.Failure("Format 1920x1080 expected");

            if (!int.TryParse(parts[1], out var h))
                return Result.Failure("Format 1920x1080 expected");

            return FromScalar(w, h);
        }

        public static Result<Resolution, string> FromScalar(int width, int height)
        {
            if (width <= 0 || width > ushort.MaxValue)
                return Result.Failure($"Width must be between 0 and {ushort.MaxValue}");
            if (height <= 0 || height > ushort.MaxValue) 
                return Result.Failure($"Height must be between 0 and {ushort.MaxValue}");

            return Result.Success(new Resolution((ushort) width, (ushort) height));
        }

        private Resolution(ushort width, ushort height)
        {
            Width = width;
            Height = height;
        }

        public Resolution Downscale(AspectRatio ratio)
        {
            var factor = ratio.ToScaler();

            if ((ushort)(Height * factor) > Width)
                return FromScalar(Width, (ushort)Math.Round(Width / factor)).UnwrapOrThrow();
            if ((ushort)(Width / factor) > Height)
                return FromScalar((ushort)Math.Round(Height * factor), Height).UnwrapOrThrow();

            return this;
        }

        public Resolution Upscale(AspectRatio ratio)
        {
            var factor = ratio.ToScaler();

            if ((ushort)(Height * factor) > Width)
                return FromScalar((ushort)Math.Round(Height * factor), Height).UnwrapOrThrow();
            if ((ushort)(Width / factor) > Height)
                return FromScalar(Width, (ushort)Math.Round(Width / factor)).UnwrapOrThrow();

            return this;
        }

        public Resolution Max(Option<ushort> maxWidth, Option<ushort> maxHeight)
        {
            var mw = maxWidth.UnwrapOr(() => Width);
            var mh = maxHeight.UnwrapOr(() => Height);

            if (Width > mw || Height > mh)
            {
                var factor = Math.Max(Width / (float)mw, Height / (float)mh);

                return FromScalar(
                    Math.Min(mw, (ushort)Math.Round(Width / factor)), 
                    Math.Min(mh, (ushort)Math.Round(Height / factor))
                ).UnwrapOrThrow();
            }

            return this;
        }

        public override string ToString() => $"{Width}x{Height}";

        protected override object GetEqualityFields() => new { Width, Height };
    }
}
