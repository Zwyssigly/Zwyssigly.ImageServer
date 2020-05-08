using System;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{

    public class AspectRatio : SimpleValueObject<AspectRatio>
    {
        public static readonly AspectRatio Square = new AspectRatio(1, 1);

        public static readonly AspectRatio Landscape_4_3 = new AspectRatio(4, 3);
        public static readonly AspectRatio Landscape_16_9 = new AspectRatio(16, 9);

        public static readonly AspectRatio Portrait_3_4 = new AspectRatio(3, 4);
        public static readonly AspectRatio Portrait_9_16 = new AspectRatio(9, 16);

        private readonly ushort _width;
        private readonly ushort _height;

        public static Result<AspectRatio, string> FromString(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return Result.Failure("Can not create aspect ratio from empty string");

            var parts = raw.Split(':');
            if (parts.Length != 2)
                return Result.Failure("Format 16:9 expected");

            if (!int.TryParse(parts[0], out var w))
                return Result.Failure("Format 16:9 expected");

            if (!int.TryParse(parts[1], out var h))
                return Result.Failure("Format 16:9 expected");

            return FromScalar(w, h);
        }

        public static AspectRatio FromResolution(Resolution resolution)
        {
            static int gcd(int a, int b)
            {
                while (a != b)
                {
                    if (a < b)
                        b -= a;
                    else
                        a -= b;
                }
                return a;
            }

            var divisor = gcd(resolution.Width, resolution.Height);

            return FromScalar((ushort)(resolution.Width / divisor), (ushort)(resolution.Height / divisor))
                .UnwrapOrThrow();
        }

        public static Result<AspectRatio, string> FromScalar(int width, int height)
        {
            if (width <= 0 || width > ushort.MaxValue)
                return Result.Failure($"Width must be between 0 and {ushort.MaxValue}");
            if (height <= 0 || height > ushort.MaxValue)
                return Result.Failure($"Height must be between 0 and {ushort.MaxValue}");

            return Result.Success(new AspectRatio((ushort)width, (ushort)height));
        }

        private AspectRatio(ushort width, ushort height)
        {
            _width = width;
            _height = height;
        }

        public override string ToString() => $"{_width}:{_height}";

        public float ToScaler() => _width / (float)_height;

        protected override object GetEqualityFields() => new { _width, _height };
    }
}
