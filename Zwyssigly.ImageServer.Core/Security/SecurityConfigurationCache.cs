using System.Collections.Concurrent;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Security
{
    public class SecurityConfigurationCache : ISecurityConfigurationStorage
    {
        private readonly ISecurityConfigurationRepository _repository;
        private readonly Option<SecurityConfiguration> _initialLogin;

        private readonly ConcurrentDictionary<Name, SecurityConfiguration> _cache = new ConcurrentDictionary<Name, SecurityConfiguration>();
        private Option<SecurityConfiguration> _globalCache;

        public SecurityConfigurationCache(ISecurityConfigurationRepository repository, InitialLoginOptions initialLoginOptions)
        {
            _repository = repository;
            _initialLogin = Name.FromString(initialLoginOptions.Username)
                .AndThen(name => AccountConfiguration.New(
                    name,
                    AccountType.Basic,
                    Option.Some(initialLoginOptions.Password),
                    Permission.All.ToValueObjectSet()
                ))
                .AndThen(a => SecurityConfiguration.New(new[] { a }))
                .Success;
        }

        public async Task<Result<Unit, Error>> Delete(Name name)
        {
            var result = await _repository.Delete(name);
            result.IfSuccess(_ => _cache.TryRemove(name, out var _));
            return result;
        }

        public async Task<Result<SecurityConfiguration, Error>> Get(Name galleryName)
        {
            if (_cache.TryGetValue(galleryName, out var configuration))
                return Result.Success(configuration);
            
            var result = await _repository.Get(galleryName);
            result.IfSuccess(c => _cache.TryAdd(galleryName, c));
            return result;
        }

        public Task<Result<SecurityConfiguration, Error>> GetGlobal()
        {
            return _globalCache.Match(
                c => Task.FromResult(Result.Success<SecurityConfiguration, Error>(c)),
                async () =>
                {
                    var result = await _repository.GetGlobal();
                    result.IfSuccess(c => _globalCache = _globalCache.OrThen(() => Option.Some(c)));
                    result.IfFailure(e =>
                    {
                        if (e.Code == ErrorCode.NoSuchRecord)
                        {
                            _globalCache = _globalCache.OrThen(() => _initialLogin);
                            _globalCache.IfSome(s => result = Result.Success(s));
                        }
                    });
                    return result;
                });
        }

        public async Task<Result<Unit, Error>> Persist(Name galleryName, SecurityConfiguration configuration)
        {
            var result = await _repository.Persist(galleryName, configuration);
            result.Match(_ => _cache.AddOrUpdate(galleryName, configuration, (_, __) => configuration), _ => _cache.TryRemove(galleryName, out var _));
            return result;
        }

        public async Task<Result<Unit, Error>> PersistGlobal(SecurityConfiguration configuration)
        {
            var result = await _repository.PersistGlobal(configuration);
            result.Match(_ => _globalCache = Option.Some(configuration), _ => _globalCache = Option.None());
            return result;
        }
    }

}
