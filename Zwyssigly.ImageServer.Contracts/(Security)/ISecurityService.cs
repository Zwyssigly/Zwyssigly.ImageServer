using System;
using System.Text;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public interface ISecurityService
    {
        Task<Result<SecurityConfiguration, Error>> GetAsync(string galleryName);
        Task<Result<SecurityConfiguration, Error>> GetGlobalAsync();

        Task<Result<Unit, Error>> ConfigureAsync(string galleryName, SecurityConfiguration configuration);
        Task<Result<Unit, Error>> ConfigureGlobalAsync(SecurityConfiguration configuration);

        Task<Result<Unit, Error>> DeleteAsync(string galleryName);
    }
}
