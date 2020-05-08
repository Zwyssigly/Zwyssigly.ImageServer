using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Security;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.MongoDb
{
    public partial class SecurityConfigurationRepository : ISecurityConfigurationRepository
    {
        private readonly IMongoCollection<SecuritySnapshot> _collection;
        private readonly string _globalId = "__GLOBAL__";

        public SecurityConfigurationRepository(MongoDbClient client)
        {
            _collection = client.Security;
        }

        public async Task<Result<Unit, Error>> Delete(Name name)
        {
            await _collection.DeleteOneAsync(s => s.GalleryName == name.ToString());
            return Result.Unit();
        }

        public async Task<Result<SecurityConfiguration, Error>> Get(Name name)
        {
            var cursor = await _collection.FindAsync(s => s.GalleryName == name.ToString());
            var snapshots = await cursor.ToListAsync();

            return snapshots.Count == 1
                ? Result.Success<SecurityConfiguration, Error>(ToConfiguration(snapshots.Single()))
                : Result.Failure<SecurityConfiguration, Error>(ErrorCode.NoSuchRecord);
        }

        public async Task<Result<SecurityConfiguration, Error>> GetGlobal()
        {
            var cursor = await _collection.FindAsync(s => s.GalleryName == _globalId);
            var snapshots = await cursor.ToListAsync();

            return snapshots.Count == 1
                ? Result.Success<SecurityConfiguration, Error>(ToConfiguration(snapshots.Single()))
                : Result.Failure<SecurityConfiguration, Error>(ErrorCode.NoSuchRecord);
        }

        public async Task<Result<Unit, Error>> Persist(Name name, SecurityConfiguration configuration)
        {
            var snapshot = ToSnapshot(name.ToString(), configuration);
            await _collection.ReplaceOneAsync(f => f.GalleryName == snapshot.GalleryName, snapshot, new ReplaceOptions { IsUpsert = true });
            return Result.Unit();
        }

        public async Task<Result<Unit, Error>> PersistGlobal(SecurityConfiguration configuration)
        {
            var snapshot = ToSnapshot(_globalId, configuration);
            await _collection.ReplaceOneAsync(f => f.GalleryName == snapshot.GalleryName, snapshot, new ReplaceOptions { IsUpsert = true });
            return Result.Unit();
        }

        private SecurityConfiguration ToConfiguration(SecuritySnapshot snapshot)
        {
            return SecurityConfiguration.New(
                accounts: snapshot.Accounts.Select(r => AccountConfiguration.New(
                    name: Name.FromString(r.Name).UnwrapOrThrow(),
                    type: (AccountType)Enum.Parse(typeof(AccountType), r.Type, true),
                    password: r.Password?.Length > 0 ? Option.Some(r.Password) : Option.None(),
                    permissions: r.Permissions.Select(Permission.FromString).SelectSuccess().ToValueObjectSet()
                )).SelectSuccess().ToArray()
            ).UnwrapOrThrow();
        }

        private SecuritySnapshot ToSnapshot(string name, SecurityConfiguration configuration)
        {
            return new SecuritySnapshot
            {
                GalleryName = name,
                Accounts = configuration.Accounts.Select(r => new AccountSnapshot
                {
                    Name = r.Name.ToString(),
                    Type = r.Type.ToString(),
                    Password = r.Password.UnwrapOrDefault(),
                    Permissions = r.Permissions.Select(q => q.ToString()).ToArray(),
                }).ToArray()
            };
        }
    }
}
