using System;
using System.Threading.Tasks;

namespace Zwyssigly.ImageServer
{
    public class GuidGenerator : IIdGenerator
    {
        public Task<Id> Generate() => Task.FromResult(new Id(Guid.NewGuid().ToByteArray()));
    }
}
