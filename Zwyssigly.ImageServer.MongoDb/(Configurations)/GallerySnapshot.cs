using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ConfigurationSnapshot
    {
        [BsonId]
        public string Name { get; set; }

        [BsonElement("dupl")]
        public bool AvoidDuplicates { get; set; }

        [BsonElement("sizes")]
        public SizeConfigurationSnapshot[] Sizes { get; set; }
    }
}
