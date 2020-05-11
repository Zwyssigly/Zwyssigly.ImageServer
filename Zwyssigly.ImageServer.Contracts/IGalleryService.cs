using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public interface IGalleryService
    {
        Task<Result<IReadOnlyCollection<string>, Error>> ListAsync();
        Task<Result<Unit, Error>> DeleteAsync(string galleryName);
        Task<Result<Unit, Error>> NewAsync(string galleryName);
    }
}
