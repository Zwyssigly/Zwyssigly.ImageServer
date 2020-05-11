using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ThumbnailSnapshot
    {
        [BsonId]
        public ThumbnailIdSnapshot Id { get; set; }

        [BsonElement("bin")]
        public byte[] Data { get; set; }
    }

    internal class ThumbnailIdSnapshot
    {
        [BsonElement("id")]
        public byte[] ImageId { get; set; }

        [BsonElement("t")]
        public string Tag { get; set; }
    }
}