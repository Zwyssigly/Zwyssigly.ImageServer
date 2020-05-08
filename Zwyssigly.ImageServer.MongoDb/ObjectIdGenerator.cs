using MongoDB.Bson;
using System.Threading.Tasks;

namespace Zwyssigly.ImageServer.MongoDb
{
    public class ObjectIdGenerator : IIdGenerator
    {
        public Task<Id> Generate()
        {
            return Task.FromResult(new Id(ObjectId.GenerateNewId().ToByteArray()));
        }
    }
}
