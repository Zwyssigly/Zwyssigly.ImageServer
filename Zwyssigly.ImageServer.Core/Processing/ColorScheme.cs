using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer.Processing
{
    public class ColorScheme : SimpleValueObject<ColorScheme>
    {
        public Color FillColor { get; }
        public Color EdgeColor { get; }

        public ColorScheme(Color fillColor, Color edgeColor)
        {
            FillColor = fillColor;
            EdgeColor = edgeColor;
        }

        protected override object GetEqualityFields() => new { FillColor, EdgeColor };

        public override string ToString() => $"fill: {FillColor}, edge: {EdgeColor}";
    }
}
