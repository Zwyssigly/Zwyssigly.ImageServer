using System.Text.RegularExpressions;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Name : SimpleValueObject<Name>
    {
        private static readonly Regex _validator = new Regex("^[a-zA-Z][a-zA-Z0-9_\\-]*$", RegexOptions.Multiline);
        
        public static Result<Name, Error> FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure(Error.ValidationError("Name must consists at least with one character!"));

            if (!_validator.IsMatch(value))
                return Result.Failure(Error.ValidationError("Invalid character in name! Only letters, numbers, dash and underscore is allowed."));

            return Result.Success(new Name(value));
        }

        private readonly string _value;

        public Name(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;
        protected override object GetEqualityFields() => _value;
    }
}
