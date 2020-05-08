using MongoDB.Bson.Serialization.Attributes;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class SizeConfigurationSnapshot
    {
        [BsonElement("tag")]
        public string Tag { get; set; }

        [BsonElement("width")]
        public ushort? MaxWidth { get; set; }
        [BsonElement("height")]
        public ushort? MaxHeight { get; set; }

        [BsonElement("crop")]
        public CropConfigurationSnapshot? Crop { get; set; }

        [BsonElement("format")]
        public string Format { get; set; }

        [BsonElement("quality")]
        public float Quality { get; set; }
    }
}
