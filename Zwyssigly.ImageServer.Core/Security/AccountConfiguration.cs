using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Security
{
    public class AccountConfiguration
    {
        public Name Name { get; }
        public AccountType Type { get; }
        public Option<string> Password { get; }
        public ValueObjectSet<Permission> Permissions { get; }

        public static Result<AccountConfiguration, Error> New(Name name, AccountType type, Option<string> password, ValueObjectSet<Permission> permissions)
        {
            if (permissions.Count == 0)
                return Result.Failure(Error.ValidationError("An account requires at least one permission"));

            if (type == AccountType.Anonymous)
            {
                if (password.IsSome)
                    return Result.Failure(Error.ValidationError("Anonymous account can not have an password"));
                if (name.ToString() != "Anonymous")
                    return Result.Failure(Error.ValidationError("Anonymous account must be called 'Anonymous'"));
            }

            if (type == AccountType.Basic)
            {
                if (password.IsNone)
                    return Result.Failure(Error.ValidationError("Basic account must have a password"));

                if (password.UnwrapOrDefault().Length < 4)
                    return Result.Failure(Error.ValidationError("Password must have a least 4 characters"));
            }

            return Result.Success(new AccountConfiguration(name, type, password, permissions));
        }

        private AccountConfiguration(Name name, AccountType type, Option<string> password, ValueObjectSet<Permission> permissions)
        {
            Name = name;
            Type = type;
            Password = password;
            Permissions = permissions;
        }
    }

    public enum AccountType
    {
        Anonymous,
        Basic
    }
}
