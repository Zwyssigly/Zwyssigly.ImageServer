using System.Collections.Generic;
using System.Linq;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Security
{
    public class SecurityConfiguration
    {
        public static readonly SecurityConfiguration Default = New(new AccountConfiguration[0]).UnwrapOrThrow();

        public IReadOnlyCollection<AccountConfiguration> Accounts { get; }

        public static Result<SecurityConfiguration, Error> New(IReadOnlyCollection<AccountConfiguration> accounts)
        {
            if (accounts.Select(a => a.Name).Distinct().Count() < accounts.Count)
                return Result.Failure(Error.ValidationError("Accounts must have unique names"));

            return Result.Success(new SecurityConfiguration(accounts));
        }

        private SecurityConfiguration(IReadOnlyCollection<AccountConfiguration> accounts)
        {
            Accounts = accounts;
        }
    }
}
