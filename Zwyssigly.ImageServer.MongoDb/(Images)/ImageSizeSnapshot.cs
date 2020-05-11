using MongoDB.Bson.Serialization.Attributes;
using System.Globalization;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ImageSizeSnapshot
    {
        [BsonElement("ratio")]
        public string AspectRatio { get; set; }

        [BsonElement("w")]
        public ushort Width { get; set; }

        [BsonElement("h")]
        public ushort Height { get; set; }

        [BsonElement("tag")]
        public string Tag { get; set; }

        [BsonElement("fmt")]
        public string ImageFormat { get; set; }

        [BsonElement("crop")]
        public string CropStrategy { get; set; }

        [BsonElement("q")]
        public float Quality { get; set; }

        [BsonElement("dupl")]
        public string DuplicateOf { get; set; }
    }
}