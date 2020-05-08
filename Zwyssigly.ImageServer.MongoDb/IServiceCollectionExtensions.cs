using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zwyssigly.ImageServer.Configuration;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.MongoDb
{
    public static class IServiceCollectionExtensions
    {
        public static void AddImageServerMongoDb(this IServiceCollection collection, MongoDbOptions options)
        {
            collection.AddSingleton(options);
            collection.AddSingleton<MongoDbClient>();
            collection.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
            collection.AddSingleton<IImageRepository, ImageRepository>();
            collection.AddSingleton<IThumbnailRepository, ThumbnailRepository>();
            collection.AddSingleton<ISecurityConfigurationRepository, SecurityConfigurationRepository>();

            if (options.UseObjectIds)
            {
                collection.RemoveAll<IIdGenerator>();
                collection.AddSingleton<IIdGenerator, ObjectIdGenerator>();
            }
        }
    }
}
