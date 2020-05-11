using System;
using Zwyssigly.Functional;
using Zwyssigly.ValueObjects;

namespace Zwyssigly.ImageServer
{
    public class ThumbnailId : SimpleValueObject<ThumbnailId>
    {
        private const char Delimiter = '$';
        public Id ImageId { get; }
        public Name Tag { get; }

        public Option<ImageFormat> FormatHint { get; }

        public ThumbnailId(Id imageId, Name tag, Option<ImageFormat> formatHint)
        {
            ImageId = imageId;
            Tag = tag;
            FormatHint = formatHint;
        }

        protected override object GetEqualityFields() => new { ImageId, Tag };

        public override string ToString() => throw new NotSupportedException();
    }
}
