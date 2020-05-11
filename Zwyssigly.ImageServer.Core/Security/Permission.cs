using System.Collections.Generic;
using System.Linq;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Security
{
    public class PermissionTypes
    {
        public const string ThumbnailRead = "thumbnail:read";
        public const string ImageRead = "image:read";
        public const string ImageWrite = "image:write";
        public const string ConfigurationRead = "configuration:read";
        public const string ConfigurationWrite = "configuration:write";
        public const string Security = "security";
        public const string Gallery = "gallery";
    }

    public class Permission : SimpleValueObject<Permission>
    {
        public static readonly Permission ThumbnailRead =  new Permission(PermissionTypes.ThumbnailRead);
        public static readonly Permission ImageRead =  new Permission(PermissionTypes.ImageRead);
        public static readonly Permission ImageWrite =  new Permission(PermissionTypes.ImageWrite);
        public static readonly Permission ConfigurationRead =  new Permission(PermissionTypes.ConfigurationRead);
        public static readonly Permission ConfigurationWrite =  new Permission(PermissionTypes.ConfigurationWrite);
        public static readonly Permission Security =  new Permission(PermissionTypes.Security);
        public static readonly Permission Gallery = new Permission(PermissionTypes.Gallery);


        public static readonly IReadOnlyCollection<Permission> All = new[]
        {
            ThumbnailRead,
            ImageRead,
            ImageWrite,
            ConfigurationRead,
            ConfigurationWrite,
            Security,
            Gallery
        };

        private static readonly IReadOnlyDictionary<string, Permission> _lookup = All.ToDictionary(p => p.ToString());

        public static Result<Permission, Error> FromString(string value)
        {
            if (_lookup.TryGetValue(value, out var permission))
                return Result.Success(permission);

            return Result.Failure(Error.ValidationError($"No such permission as '{value}' exists"));
        }

        private readonly string _value;
        private Permission(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        protected override object GetEqualityFields() => _value;
    }
}
