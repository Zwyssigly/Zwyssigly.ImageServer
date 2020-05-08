using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ThumbnailSnapshot
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("bin")]
        public byte[] Data { get; set; }
    }
}