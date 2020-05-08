using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class CropConfigurationSnapshot
    {
        [BsonElement("strategy")]
        public string CropStrategy { get; set; }

        [BsonElement("ratio")]
        public string AspectRatio { get; set; }

        [BsonElement("color")]
        public uint? Color { get; set; }
    }
}
