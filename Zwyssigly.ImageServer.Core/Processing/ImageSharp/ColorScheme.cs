namespace Zwyssigly.ImageServer.Processing.ImageSharp
{
    internal class ColorScheme
    {
        public Color FillColor { get; }
        public Color EdgeColor { get; }

        public ColorScheme(Color fillColor, Color edgeColor)
        {
            FillColor = fillColor;
            EdgeColor = edgeColor;
        }
    }
}
