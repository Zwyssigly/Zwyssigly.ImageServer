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

        public async Task<Result<Unit, Contracts.Error>> ConfigureAsync(string galleryName, Contracts.SecurityConfiguration configuration)
        {
            var result = await _storage.Persist(Name.FromString(galleryName).UnwrapOrThrow(), ToConfiguration(configuration));
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Unit, Contracts.Error>> ConfigureGlobalAsync(Contracts.SecurityConfiguration configuration)
        {
            var result = await _storage.PersistGlobal(ToConfiguration(configuration));
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.SecurityConfiguration, Contracts.Error>> GetAsync(string galleryName)
        {
            var config = await _storage.Get(Name.FromString(galleryName).UnwrapOrThrow());
            return config.Map(ToModel, ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.SecurityConfiguration, Contracts.Error>> GetGlobalAsync()
        {
            var config = await _storage.GetGlobal();
            return config.Map(ToModel, ErrorExtensions.ToContract);
        }


        public async Task<Result<Unit, Contracts.Error>> DeleteAsync(string galleryName)
        {
            var result = await _storage.Delete(Name.FromString(galleryName).UnwrapOrThrow());
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        private Security.SecurityConfiguration ToConfiguration(Contracts.SecurityConfiguration model)
        {
            return Security.SecurityConfiguration.New(
                accounts: model.Accounts.Select(a => Security.AccountConfiguration.New(
                    name: Name.FromString(a.Name).UnwrapOrThrow(),
                    type: (AccountType)Enum.Parse(typeof(AccountType), a.Type, true),
                    password: a.Password?.Length > 0 ? Option.Some(a.Password) : Option.None(),
                    permissions: a.Permissions.Select(r => Permission.FromString(r).UnwrapOrThrow()).ToValueObjectSet()
                ).UnwrapOrThrow()).ToArray()
            ).UnwrapOrThrow();
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
