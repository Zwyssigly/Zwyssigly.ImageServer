namespace Zwyssigly.ImageServer.Thumbnails.Disk
{
    public class DiskOptions
    {
        public string Directory { get; set; } = "";
        public char TagDelimiter { get; set; } = '$';
        public bool FileExtensions { get; set; } = false;
    }
}
