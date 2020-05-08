using System.Collections.Generic;

namespace Zwyssigly.ImageServer.Contracts
{
    public class SecurityConfiguration
    {
        public IReadOnlyCollection<AccountConfiguration> Accounts { get; }

        public SecurityConfiguration(IReadOnlyCollection<AccountConfiguration> accounts)
        {
            Accounts = accounts;
        }
    }
}
