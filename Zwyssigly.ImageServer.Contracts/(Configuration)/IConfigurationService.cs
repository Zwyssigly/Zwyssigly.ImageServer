using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public interface IConfigurationService
    {
        Task<Result<Unit, Error>> SetAsync(string gallery, ProcessingConfiguration configuration);
        Task<Result<ProcessingConfiguration, Error>> GetAsync(string gallery);
    }
}
