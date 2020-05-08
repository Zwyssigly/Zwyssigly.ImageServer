using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer
{

    public interface IThumbnailRepository
    {
        Task<Result<Unit, Error>> Insert(Name gallery, IEnumerable<Thumbnail> thumbnails);
        Task<Result<Thumbnail, Error>> Get(Name gallery, ThumbnailId id);
        Task<Result<Unit, Error>> Delete(Name gallery, IEnumerable<ThumbnailId> ids);
        Task<Result<Unit, Error>> Truncate(Name gallery);
    }
}
