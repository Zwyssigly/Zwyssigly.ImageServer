using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Color : SimpleValueObject<Color>
    {
        public static readonly Color Black = new Color(0);

        private readonly uint _value;

        public static Result<Color, string> FromString(string value)
        {
            if (value.Length == 6 && uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var number))
                return Result.Success(new Color(number));

            return Result.Failure("Color expect as hex, e.g.: ff00ff");
        }

        public Color(uint value)
        {
            _value = value;
        }

        public uint ToScalar() => _value;

        public override string ToString() => _value.ToString("x6");

        protected override object GetEqualityFields() => _value;
    }
}
