using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class AccountSnapshot
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("pw")]
        public string Password { get; set; }

        [BsonElement("perms")]
        public string[] Permissions { get; set; }

    }
}
