using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Images
{

    public interface IImageRepository
    {
        Task<Result<Unit, Error>> Insert(Name name, IEnumerable<Image> images);
        Task<Result<Unit, Error>> Update(Name name, IEnumerable<Image> images);
        Task<Result<IReadOnlyCollection<Image>, Error>> Get(Name name, IEnumerable<Id> ids);
        Task<Result<Unit, Error>> Delete(Name name, IEnumerable<Id> ids);
        Task<Result<Unit, Error>> Truncate(Name name);
        Task<Result<Page<Image>, Error>> List(Name name, uint skip, uint take);
    }
}
