using System.Collections.Generic;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.Processing
{
    public interface IImageFactory
    {
        Task<Result<(Image Image, IReadOnlyCollection<Thumbnail> Thumbnails), Error>>
            Create(Id id, uint rowVersion, Option<byte[]> meta, byte[] raw, ProcessingConfiguration configuration);
    }
}
