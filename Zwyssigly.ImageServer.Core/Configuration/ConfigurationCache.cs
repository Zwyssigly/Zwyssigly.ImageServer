using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Configuration
{
    class ConfigurationCache : IConfigurationStorage
    {
        private readonly IConfigurationRepository _repository;
        private readonly ConcurrentDictionary<Name, ProcessingConfiguration> _cache = new ConcurrentDictionary<Name, ProcessingConfiguration>();

        public ConfigurationCache(IConfigurationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Unit, Error>> Delete(Name name)
        {
            _cache.TryRemove(name, out var _);
            var result = await _repository.Delete(name);
            result.IfSuccess(_ => _cache.TryRemove(name, out var _));
            return result;
        }

        public async Task<Result<ProcessingConfiguration, Error>> Get(Name name)
        {
            if (!_cache.TryGetValue(name, out var config))
            {
                var result = await _repository.Get(name);
                result.IfSuccess(config => _cache.TryAdd(name, config));
                return result;
            }
            return Result.Success(config);
        }

        public Task<Result<IReadOnlyCollection<Name>, Error>> List()
        {
            return _repository.List();
        }

        public async Task<Result<Unit, Error>> Persist(Name name, ProcessingConfiguration configuration)
        {
            _cache.AddOrUpdate(name, configuration, (_, __) => configuration);
            var result = await _repository.Persist(name, configuration);
            result.IfSuccess(_ => _cache.TryRemove(name, out var _));
            return result;
        }
    }
}
