
using System;

namespace Zwyssigly.ImageServer.Contracts
{
    public class Image
    {
        public string Id { get; }
        public uint RowVersion { get; }
        public byte[]? Meta { get; }
        public string EdgeColor { get; }
        public string FillColor { get; }
        public DateTime UploadedAt { get; }
        public byte[] Md5 { get; }
        public ImageSize[] Sizes { get; }

        public Image(string id, uint rowVersion, byte[]? meta, DateTime uploadedAt, string fillColor, string edgeColor, byte[] md5, ImageSize[] sizes)
        {
            Id = id;
            RowVersion = rowVersion;
            Meta = meta;
            UploadedAt = uploadedAt;
            FillColor = fillColor;
            EdgeColor = edgeColor;
            Md5 = md5;
            Sizes = sizes;
        }
    }
}
