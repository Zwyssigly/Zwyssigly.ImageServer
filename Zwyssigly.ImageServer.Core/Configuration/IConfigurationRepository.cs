using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Configuration
{
    public interface IConfigurationRepository
    {
        Task<Result<IReadOnlyCollection<Name>, Error>> List();
        Task<Result<ProcessingConfiguration, Error>> Get(Name name);

        Task<Result<Unit, Error>> Delete(Name name);
        Task<Result<Unit, Error>> Persist(Name name, ProcessingConfiguration configuration);
    }
}
