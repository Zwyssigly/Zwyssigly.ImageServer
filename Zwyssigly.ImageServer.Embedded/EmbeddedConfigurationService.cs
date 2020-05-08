using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Embedded
{
    class EmbeddedConfigurationService : IConfigurationService
    {
        private readonly IConfigurationStorage _storage;
        private readonly IImageRepository _imageRepository;
        private readonly IThumbnailRepository _thumbnailRepository;
        private readonly ISecurityConfigurationStorage _securityStorage;

        public EmbeddedConfigurationService(
            IConfigurationStorage storage,
            IImageRepository imageRepository,
            IThumbnailRepository thumbnailRepository,
            ISecurityConfigurationStorage securityStorage)
        {
            _storage = storage;
            _imageRepository = imageRepository;
            _thumbnailRepository = thumbnailRepository;
            _securityStorage = securityStorage;
        }

        public async Task<Result<Unit, Contracts.Error>> ConfigureAsync(string gallery, Contracts.ProcessingConfiguration configuration)
        {
            var result = await _storage.Persist(Name.FromString(gallery).UnwrapOrThrow(), FromContract(configuration));
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Unit, Contracts.Error>> DeleteAsync(string gallery)
        {
            var galleryName = Name.FromString(gallery).UnwrapOrThrow();

            var result = await _imageRepository.Truncate(galleryName)
                .AndThenAsync(_ => _thumbnailRepository.Truncate(galleryName))
                .AndThenAsync(_ => _storage.Delete(galleryName))
                .AndThenAsync(_ => _securityStorage.Delete(galleryName));

            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.ProcessingConfiguration, Contracts.Error>> GetAsync(string gallery)
        {
            var result = await _storage.Get(Name.FromString(gallery).UnwrapOrThrow());
            return result.Map(ToContract, ErrorExtensions.ToContract);
        }

        public async Task<Result<IReadOnlyCollection<string>, Contracts.Error>> ListAsync()
        {
            var result = await _storage.List();
            return result.Map(
                s => (IReadOnlyCollection<string>) s.Select(g => g.ToString()).ToArray(),
                ErrorExtensions.ToContract);
        }

        private static Contracts.ProcessingConfiguration ToContract(Configuration.ProcessingConfiguration config)
        {
            return new Contracts.ProcessingConfiguration(
                config.AvoidDuplicates,
                config.Sizes.Select(s => new Contracts.SizeConfiguration(
                    s.Tag.ToString(),
                    s.MaxWidth.ToNullable(),
                    s.MaxHeight.ToNullable(),
                    s.Crop.Map(c => new Contracts.CropConfiguration(
                        c.AspectRatio.ToString(),
                        c.CropStrategy.ToString(),
                        c.BackgroundColor.Map(bc => bc.ToString()).UnwrapOrDefault()
                    )).UnwrapOrDefault(),
                    s.Format.FileExtension,
                    s.Quality.ToScaler()
                )).ToArray()
            );
        }

        private static Configuration.ProcessingConfiguration FromContract(Contracts.ProcessingConfiguration value)
        {
            return Configuration.ProcessingConfiguration.New(
                value.AvoidDuplicates,
                value.Sizes.Select(s => new Configuration.SizeConfiguration(
                    Name.FromString(s.Tag).UnwrapOrThrow(),
                    s.MaxWidth.ToOption(),
                    s.MaxHeight.ToOption(),
                    s.Crop != null
                        ? Option.Some(new Configuration.CropConfiguration(
                            AspectRatio.FromString(s.Crop.AspectRatio).UnwrapOrThrow(),
                            (CropStrategy)Enum.Parse(typeof(CropStrategy), s.Crop.CropStrategy, true),
                            s.Crop.Color != null
                                ? Option.Some(Color.FromString(s.Crop.Color).UnwrapOrThrow())
                                : Option.None()
                        )) : Option.None(),
                    Quality.FromScalar(s.Quality).UnwrapOrThrow(),
                    ImageFormat.FromExtension(s.Format).UnwrapOrThrow()
                )).ToArray()
            ).UnwrapOrThrow();
        }
    }
}
