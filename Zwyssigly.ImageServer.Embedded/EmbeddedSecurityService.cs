using System;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Embedded
{
    public class EmbeddedSecurityService : ISecurityService
    {
        private readonly ISecurityConfigurationStorage _storage;

        public EmbeddedSecurityService(ISecurityConfigurationStorage storage)
        {
            _storage = storage;
        }

        public async Task<Result<Unit, Contracts.Error>> SetAsync(string galleryName, Contracts.SecurityConfiguration configuration)
        {
            var result = await Name.FromString(galleryName)
                .AndThenAsync(gallery => ToConfiguration(configuration)
                .AndThenAsync(config => _storage.Persist(gallery, config)));
            
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Unit, Contracts.Error>> SetGlobalAsync(Contracts.SecurityConfiguration configuration)
        {
            var result = await ToConfiguration(configuration).AndThenAsync(config => _storage.PersistGlobal(config));
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.SecurityConfiguration, Contracts.Error>> GetAsync(string galleryName)
        {
            var config = await Name.FromString(galleryName).AndThenAsync(name => _storage.Get(name));
            return config.Map(ToModel, ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.SecurityConfiguration, Contracts.Error>> GetGlobalAsync()
        {
            var config = await _storage.GetGlobal();
            return config.Map(ToModel, ErrorExtensions.ToContract);
        }

        private Result<Security.SecurityConfiguration, Error> ToConfiguration(Contracts.SecurityConfiguration model)
        {
            var accountsResult = model.Accounts.Select(a =>
            {
                if (!Enum.TryParse<AccountType>(a.Type, true, out var type))
                    return Result.Failure<Security.AccountConfiguration, Error>(Error.ValidationError($"Unknown account type {a.Type}"));

                var permissionResults = a.Permissions.Select(Permission.FromString).ToArray();
                if (permissionResults.Any(r => r.IsFailure))
                    return Result.Failure(permissionResults.SelectFailure().First());

                return Name.FromString(a.Name).AndThen(name => Security.AccountConfiguration.New(
                    name,
                    type,
                    a.Password?.Length > 0 ? Option.Some(a.Password) : Option.None(),
                    permissionResults.SelectSuccess().ToValueObjectSet()));

            }).Railway();

            return accountsResult.AndThen(accounts => Security.SecurityConfiguration.New(accounts));
        }

        private Contracts.SecurityConfiguration ToModel(Security.SecurityConfiguration configuration)
        {
            return new Contracts.SecurityConfiguration(
                accounts: configuration.Accounts.Select(a => new Contracts.AccountConfiguration(
                    a.Name.ToString(),
                    a.Type.ToString(),
                    a.Password.UnwrapOrDefault(),
                    a.Permissions.Select(r => r.ToString()).ToArray()
               )).ToArray()
            );
        }
    }
}
