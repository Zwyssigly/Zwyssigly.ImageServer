namespace Zwyssigly.ImageServer
{
    public class Thumbnail
    {
        public ThumbnailId ThumbnailId { get; }
        public byte[] Data { get; }        

        public Thumbnail(ThumbnailId thumbnailId, byte[] data)
        {
            ThumbnailId = thumbnailId;
            Data = data;            
        }
    }
}
