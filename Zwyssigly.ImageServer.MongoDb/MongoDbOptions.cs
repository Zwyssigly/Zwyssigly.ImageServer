using MongoDB.Driver;

namespace Zwyssigly.ImageServer.MongoDb
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; } = "";
        public string CollectionNamePrefix { get; set; } = "";
        public string ThumbnailCollectionPrefix { get; set; } = "thumbnails_";
        public string ImageCollectionPrefix { get; set; } = "images_";
        public string ConfigurationCollectionName { get; set; } = "configurations";
        public string SecurityCollectionName { get; set; } = "security";

        public bool UseObjectIds { get; set; } = false;
    }

    public class MongoDbClient
    {
        public MongoDbOptions Options { get; }

        public IMongoDatabase Database { get; }

        internal IMongoCollection<ConfigurationSnapshot> Configurations { get; }

        internal IMongoCollection<SecuritySnapshot> Security { get; }

        internal IMongoCollection<ImageSnapshot> Images(Name galleryName) 
            => Database.GetCollection<ImageSnapshot>(ImagesCollectionName(galleryName));

        internal IMongoCollection<ThumbnailSnapshot> Thumbnails(Name galleryName)
            => Database.GetCollection<ThumbnailSnapshot>(ThumbnailsCollectionName(galleryName));

        internal string ImagesCollectionName(Name galleryName)
            => Options.CollectionNamePrefix + Options.ImageCollectionPrefix + galleryName;

        internal string ThumbnailsCollectionName(Name galleryName)
             => Options.CollectionNamePrefix + Options.ThumbnailCollectionPrefix + galleryName;

        public MongoDbClient(MongoDbOptions options)
        {
            Options = options;

            var url = MongoUrl.Create(options.ConnectionString);
            var client = new MongoClient(url);
            Database = client.GetDatabase(url.DatabaseName);

            Configurations = Database.GetCollection<ConfigurationSnapshot>(options.CollectionNamePrefix + options.ConfigurationCollectionName);
            Security = Database.GetCollection<SecuritySnapshot>(options.CollectionNamePrefix + options.SecurityCollectionName);
        }
    }
}
