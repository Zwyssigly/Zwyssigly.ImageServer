using System;
using System.Collections.Generic;
using System.Text;

namespace Zwyssigly.ImageServer.Contracts
{
    public class ResolvedThumbnail
    {
        public string Id { get; }
        public uint RowVersion { get; }
        public string FillColor { get; }
        public string EdgeColor { get; }
        public string Tag { get; }
        public string Format { get; }
        public ushort Width { get; }
        public ushort Height { get; }

        public ResolvedThumbnail(string id, uint rowVersion, string fillColor, string edgeColor, string tag, string format, ushort width, ushort height)
        {
            Id = id;
            RowVersion = rowVersion;
            FillColor = fillColor;
            EdgeColor = edgeColor;
            Tag = tag;
            Format = format;
            Width = width;
            Height = height;
        }
    }
}
