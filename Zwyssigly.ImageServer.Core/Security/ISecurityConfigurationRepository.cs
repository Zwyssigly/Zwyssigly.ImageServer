using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Security
{
    public interface ISecurityConfigurationRepository
    {
        public Task<Result<Unit, Error>> Persist(Name galleryName, SecurityConfiguration configuration);
        public Task<Result<Unit, Error>> PersistGlobal(SecurityConfiguration configuration);

        public Task<Result<SecurityConfiguration, Error>> Get(Name galleryName);
        public Task<Result<SecurityConfiguration, Error>> GetGlobal();

        public Task<Result<Unit, Error>> Delete(Name name);
    }

}
