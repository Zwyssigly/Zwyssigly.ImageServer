using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ImageSnapshot
    {
        [BsonId]
        public byte[] Id { get; set; }

        [BsonElement("version")]
        public uint RowVersion { get; set; }

        [BsonElement("ts")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UploadedAt { get; set; }

        [BsonElement("meta")]
        public byte[]? Meta { get; set; }

        [BsonElement("md5")]
        public byte[] Md5 { get; set; }

        [BsonElement("fill")]
        public uint FillColor { get; set; }

        [BsonElement("edge")]
        public uint EdgeColor { get; set; }

        [BsonElement("sizes")]
        public ImageSizeSnapshot[] Sizes { get; set; }
    }
}