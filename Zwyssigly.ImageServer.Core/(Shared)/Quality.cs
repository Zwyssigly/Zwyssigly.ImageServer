using System;
using System.Collections.Generic;
using System.Text;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Quality : SimpleValueObject<Quality>
    {
        public static Result<Quality, Error> FromScalar(float value)
        {
            if (value <= 0f || value > 1f)
                return Result.Failure(Error.ValidationError("Quality must be between 0.0 and 1.0"));

            return Result.Success(new Quality(value));
        }

        private readonly float _value;

        private Quality(float value) => _value = value;

        public override string ToString() => _value.ToString("0.00");

        public float ToScaler() => _value;

        protected override object GetEqualityFields() => _value;
    }
}
