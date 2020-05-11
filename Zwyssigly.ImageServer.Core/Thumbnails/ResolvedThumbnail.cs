namespace Zwyssigly.ImageServer.Thumbnails
{
    public class ResolvedThumbnail
    {
        public Id Id { get; }
        public uint RowVersion { get; }
        public Color FillColor { get; }
        public Color EdgeColor { get; }
        public Name Tag { get; }
        public ImageFormat Format { get; }
        public Resolution Resolution { get; }

        public ResolvedThumbnail(Id id, uint rowVersion, Color fillColor, Color edgeColor, Name tag, ImageFormat format, Resolution resolution)
        {
            Id = id;
            RowVersion = rowVersion;
            FillColor = fillColor;
            EdgeColor = edgeColor;
            Tag = tag;
            Format = format;
            Resolution = resolution;
        }
    }
}
