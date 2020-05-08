using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class Md5 : SimpleValueObject<Md5>
    {
        public static Result<Md5, string> FromBase64(string value)
        {
            var bytes = Convert.FromBase64String(value);

            return FromByteArray(bytes);
        }

        public static Result<Md5, string> ComputeHash(byte[] value)
        {
            if (value.Length < 1)
            {
                return Result.Failure("Byte array must consist a least of one byte!");
            }

            using (var algorithm = System.Security.Cryptography.MD5.Create())
            {
                var hash = algorithm.ComputeHash(value);
                return FromByteArray(hash);
            }
        }

        public static Result<Md5, string> FromByteArray(byte[] value)
        {
            if (value.Length != 16)
            {
                return Result.Failure("Byte array must consist of 16 bytes");
            }

            return Result.Success(new Md5(value));
        }

        private readonly byte[] _hash;

        private Md5(byte[] hash)
        {
            _hash = hash;
        }

        public override string ToString() => ToBase64();

        public string ToBase64() => Convert.ToBase64String(_hash);

        public byte[] ToByteArray() => _hash;

        protected override object GetEqualityFields() => ToBase64();
    }
}
