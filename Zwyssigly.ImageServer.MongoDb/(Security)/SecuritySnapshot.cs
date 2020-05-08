using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class SecuritySnapshot
    {
        [BsonId]
        public string GalleryName { get; set; }

        [BsonElement("accounts")]
        public AccountSnapshot[] Accounts { get; set; }
    }
}
