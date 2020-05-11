using Microsoft.Extensions.DependencyInjection;
using System;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Core;

namespace Zwyssigly.ImageServer.Embedded
{
    public static class IServiceCollectionExtensions
    {
        public static void AddImageServerEmbedded(this IServiceCollection collection)
        {
            collection.AddImageServerCore();

            collection.AddSingleton<IConfigurationService, EmbeddedConfigurationService>();
            collection.AddSingleton<IImageService, EmbeddedImageService>();
            collection.AddSingleton<IThumbnailService, EmbeddedThumbnailService>();
            collection.AddSingleton<ISecurityService, EmbeddedSecurityService>();
            collection.AddSingleton<IGalleryService, EmbeddedGalleryService>();
        }
    }
}
