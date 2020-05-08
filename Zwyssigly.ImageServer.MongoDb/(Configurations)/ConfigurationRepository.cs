using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Configuration;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ConfigurationRepository : IConfigurationRepository
    {
        private readonly MongoDbClient _client;

        public ConfigurationRepository(MongoDbClient client)
        {
            _client = client;
        }

        public async Task<Result<Unit, Error>> Delete(Name name)
        {
            var response = await _client.Configurations.DeleteOneAsync(g => g.Name == name.ToString());
            return response.DeletedCount == 0
                ? Result.Failure<Unit, Error>(ErrorCode.NoSuchRecord)
                : Result.Unit();
        }

        public async Task<Result<ProcessingConfiguration, Error>> Get(Name name)
        {
            var cursor = await _client.Configurations.FindAsync(g => g.Name == name.ToString());
            var snapshot = await cursor.ToListAsync();

            return snapshot.Count == 0
                ? Result.Failure<ProcessingConfiguration, Error>(ErrorCode.NoSuchRecord)
                : Result.Success(ToConfiguration(snapshot[0]));
        }

        private ProcessingConfiguration ToConfiguration(ConfigurationSnapshot snapshot)
        {
            return ProcessingConfiguration.New(
                avoidDuplicates: snapshot.AvoidDuplicates,
                sizes: snapshot.Sizes.Select(s => new SizeConfiguration(
                    tag: Name.FromString(s.Tag).UnwrapOrThrow(),
                    maxWidth: s.MaxWidth.ToOption(),
                    maxHeight: s.MaxHeight.ToOption(),
                    crop: s.Crop != null
                        ? Option.Some(new CropConfiguration(
                            aspectRatio: AspectRatio.FromString(s.Crop.AspectRatio).UnwrapOrThrow(),
                            cropStrategy: (CropStrategy)Enum.Parse(typeof(CropStrategy), s.Crop.CropStrategy, true),
                            backgroundColor: s.Crop.Color.ToOption().Map(s => new Color(s))
                        )) : Option.None(),
                    quality: Quality.FromScalar(s.Quality).UnwrapOrThrow(),
                    format: ImageFormat.FromExtension(s.Format).UnwrapOrThrow()
                )).ToArray()
            ).UnwrapOrThrow();
        }

        public async Task<Result<IReadOnlyCollection<Name>, Error>> List()
        {
            var cursor = await _client.Configurations.FindAsync(_ => true);
            var snapshots = await cursor.ToListAsync();

            return Result.Success<IReadOnlyCollection<Name>, Error>(
                snapshots.Select(s => Name.FromString(s.Name).UnwrapOrThrow()).ToArray());
        }

        public async Task<Result<Unit, Error>> Persist(Name name, ProcessingConfiguration configuration)
        {
            var snapshot = new ConfigurationSnapshot
            {
                Name = name.ToString(),
                AvoidDuplicates = configuration.AvoidDuplicates,
                Sizes = configuration.Sizes.Select(s => new SizeConfigurationSnapshot
                {
                    Tag = s.Tag.ToString(),
                    MaxWidth = s.MaxWidth.ToNullable(),
                    MaxHeight = s.MaxHeight.ToNullable(),
                    Format = s.Format.FileExtension,
                    Quality = s.Quality.ToScaler(),
                    Crop = s.Crop.Map(c => new CropConfigurationSnapshot
                    {
                        AspectRatio = c.AspectRatio.ToString(),
                        Color = c.BackgroundColor.Map(c => c.ToScalar()).ToNullable(),
                        CropStrategy = c.CropStrategy.ToString()
                    }).UnwrapOrDefault()
                }).ToArray()
            };

            await _client.Configurations.ReplaceOneAsync(g => g.Name == snapshot.Name, snapshot, new ReplaceOptions { IsUpsert = true });
            return Result.Unit();
        }
    }
}
