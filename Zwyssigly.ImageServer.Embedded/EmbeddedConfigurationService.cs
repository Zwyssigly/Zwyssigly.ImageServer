using System;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;
using Zwyssigly.ImageServer.Contracts;

namespace Zwyssigly.ImageServer.Embedded
{
    class EmbeddedConfigurationService : IConfigurationService
    {
        private readonly IConfigurationStorage _storage;

        public EmbeddedConfigurationService(IConfigurationStorage storage)
        {
            _storage = storage;
        }

        public async Task<Result<Unit, Contracts.Error>> SetAsync(string gallery, Contracts.ProcessingConfiguration configuration)
        {
            var result = await Name.FromString(gallery)
                .AndThenAsync(name => FromContract(configuration)
                .AndThenAsync(config => _storage.Persist(name, config)));
            
            return result.MapFailure(ErrorExtensions.ToContract);
        }

        public async Task<Result<Contracts.ProcessingConfiguration, Contracts.Error>> GetAsync(string gallery)
        {
            var result = await Name.FromString(gallery).AndThenAsync(name => _storage.Get(name));
            return result.Map(ToContract, ErrorExtensions.ToContract);
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

        private static Result<Configuration.ProcessingConfiguration, Error> FromContract(Contracts.ProcessingConfiguration value)
        {
            var sizesResult = value.Sizes.Select(s =>
            {
                var cropResult = Result.Success<Option<Configuration.CropConfiguration>, Error>(Option.None());
                if (s.Crop != null)
                {
                    if (Enum.TryParse<CropStrategy>(s.Crop.CropStrategy, true, out var strategy))
                    {
                        var colorResult = Result.Success<Option<Color>, Error>(Option.None());
                        if (s.Crop.Color != null)
                            colorResult = Color.FromString(s.Crop.Color).MapSuccess(c => Option.Some(c));

                        cropResult = colorResult.AndThen(color => AspectRatio.FromString(s.Crop.AspectRatio)
                            .MapSuccess(ratio => Option.Some(new Configuration.CropConfiguration(ratio, strategy, color))));
                    }
                    else cropResult = Result.Failure(Error.ValidationError($"Unknown crop strategy {s.Crop.CropStrategy}"));
                }

                return cropResult
                    .AndThen(crop => Name.FromString(s.Tag)
                    .AndThen(tag => Quality.FromScalar(s.Quality)
                    .AndThen(quality => ImageFormat.FromExtension(s.Format)
                    .MapSuccess(format => new Configuration.SizeConfiguration(
                        tag,
                        s.MaxWidth.ToOption(),
                        s.MaxHeight.ToOption(),
                        crop,
                        quality,
                        format)))));

            }).Railway();

            return sizesResult.AndThen(sizes => Configuration.ProcessingConfiguration.New(
                value.AvoidDuplicates,
                sizes
            ));
        }
    }
}
