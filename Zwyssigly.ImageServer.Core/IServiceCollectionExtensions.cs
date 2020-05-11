using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using System.Collections.Generic;
using System.Net.Http;
using Zwyssigly.ImageServer.Configuration;
using Zwyssigly.ImageServer.Images;
using Zwyssigly.ImageServer.Processing;
using Zwyssigly.ImageServer.Processing.ImageSharp;
using Zwyssigly.ImageServer.Security;
using Zwyssigly.ImageServer.Thumbnails;
using Zwyssigly.ImageServer.Thumbnails.Disk;

namespace Zwyssigly.ImageServer.Core
{
    public static class IServiceCollectionExtensions
    {
        public static void AddImageServerCore(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IClock>(SystemClock.Instance);

            collection.AddSingleton<ImageUploadService>();
            collection.AddSingleton<ThumbnailResolver>();

            collection.AddSingleton<IImageFactory, ImageFactory>();
            collection.AddSingleton<IImageProcessorFactory, ImageSharpProcessorFactory>();

            collection.AddSingleton<IConfigurationStorage, ConfigurationCache>();
            collection.AddSingleton<ISecurityConfigurationStorage, SecurityConfigurationCache>();

            collection.AddSingleton<IIdGenerator, GuidGenerator>();
            collection.AddSingleton<IHttpClientWrapper, NullHttpClientWrapper>();
        }

        public static void AddImageServerConfigurations(this IServiceCollection collection, IReadOnlyDictionary<Name, ProcessingConfiguration> configurations)
        {
            collection.RemoveAll<IConfigurationStorage>();
            collection.AddSingleton<IConfigurationStorage>(new StaticConfiguration(configurations));
        }

        public static void AddImageServerDisk(this IServiceCollection collection, DiskOptions options)
        {
            collection.AddSingleton(options);

            collection.RemoveAll<IThumbnailRepository>();
            collection.AddSingleton<IThumbnailRepository, DiskThumbnailRepository>();
        }

        public static void AddImageServerHttpClient(this IServiceCollection collection, HttpClient client)
        {
            collection.AddSingleton<IHttpClientWrapper>(new HttpClientWrapper(client));
        }
    }
}
