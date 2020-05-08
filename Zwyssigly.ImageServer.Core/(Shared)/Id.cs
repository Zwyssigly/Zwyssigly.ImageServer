using System;
using System.Linq;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Id : AbstractValueObject<Id>
    {
        public static Result<Id, Error> FromString(string value)
        {
            try
            {
                value = value.Replace('-', '+').Replace('_', '/');
                var mod = value.Length % 4;
                if (mod > 0) value = value.PadRight(value.Length - mod + 4, '=');
                return Result.Success(new Id(Convert.FromBase64String(value)));
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.ValidationError(ex.Message));
            }
        }

        private readonly byte[] _value;

        public Id(byte[] value)
        {
            _value = value;            
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _value.Aggregate((int)2166136261, (a, b) => (a ^ b) * 16777619);
            }
        }

        public override string ToString() => Convert
            .ToBase64String(_value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        public byte[] ToByteArray() => _value;

        protected override bool EqualsImpl(Id other)
            => _value.Length == other._value.Length && _value.Zip(other._value, (a, b) => a == b).All(_ => _);
    }
}
