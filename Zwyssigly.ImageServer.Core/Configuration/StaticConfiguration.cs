using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Configuration
{
    public class StaticConfiguration : IConfigurationStorage
    {
        private IReadOnlyDictionary<Name, ProcessingConfiguration> _values;

        public StaticConfiguration(IReadOnlyDictionary<Name, ProcessingConfiguration> values)
        {
            _values = values;
        }

        public Task<Result<Unit, Error>> Delete(Name name)
        {
            return Task.FromResult(Result.Failure<Unit, Error>(ErrorCode.NotSupported));
        }

        public Task<Result<ProcessingConfiguration, Error>> Get(Name name)
        {
            if (_values.TryGetValue(name, out var configuration)) 
            {
                return Task.FromResult(Result.Success<ProcessingConfiguration, Error>(configuration));
            }

            return Task.FromResult(Result.Failure<ProcessingConfiguration, Error>(ErrorCode.NoSuchRecord));
        }

        public Task<Result<IReadOnlyCollection<Name>, Error>> List()
        {
            return Task.FromResult(Result.Success<IReadOnlyCollection<Name>, Error>(_values.Keys.ToArray()));
        }

        public Task<Result<Unit, Error>> Persist(Name name, ProcessingConfiguration configuration)
        {
            return Task.FromResult(Result.Failure<Unit, Error>(ErrorCode.NotSupported));
        }
    }
}
