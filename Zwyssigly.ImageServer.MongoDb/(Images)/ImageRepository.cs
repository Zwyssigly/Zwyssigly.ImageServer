using MongoDB.Driver;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ImageRepository : IImageRepository
    {
        private readonly MongoDbClient _client;

        public ImageRepository(MongoDbClient client)
        {
            _client = client;
        }

        public async Task<Result<Unit, Error>> Delete(Name name, IEnumerable<Id> ids)
        {
            var byteIds = ids.Select(id => id.ToByteArray()).ToArray();

            var filter = Builders<ImageSnapshot>.Filter.In(i => i.Id, byteIds);
            var response = await _client.Images(name).DeleteManyAsync(filter);

            return response.DeletedCount == byteIds.Length
                 ? Result.Unit<Error>()
                 : Result.Failure<Unit, Error>(ErrorCode.NoSuchRecord);
        }

        public async Task<Result<IReadOnlyCollection<Image>, Error>> Get(Name name, IEnumerable<Id> ids)
        {
            var byteIds = ids.Select(id => id.ToByteArray()).ToArray();

            var filter = Builders<ImageSnapshot>.Filter.In(i => i.Id, byteIds);
            var cursor = await _client.Images(name).FindAsync(filter).ConfigureAwait(false);
            var snapshots = await cursor.ToListAsync().ConfigureAwait(false);

            return snapshots.Count == byteIds.Length
                ? Result.Success<IReadOnlyCollection<Image>, Error>(snapshots.Select(ToAggregate).ToArray())
                : Result.Failure<IReadOnlyCollection<Image>, Error>(ErrorCode.NoSuchRecord);
        }

        public async Task<Result<Page<Image>, Error>> List(Name name, uint skip, uint take)
        {
            var snapshots = await _client.Images(name).Find(_ => true).Skip((int)skip).Limit((int)take).ToListAsync().ConfigureAwait(false);
            var count = await _client.Images(name).CountDocumentsAsync(_ => true).ConfigureAwait(false);

            return Result.Success(new Page<Image>(snapshots.Select(ToAggregate).ToArray(), (ulong)count));

        }

        private static Image ToAggregate(ImageSnapshot snapshot)
        {
            return Image.New(
                new Id(snapshot.Id),
                snapshot.RowVersion,
                Instant.FromDateTimeUtc(snapshot.UploadedAt),
                snapshot.Meta?.Length > 0 ? Option.Some(snapshot.Meta) : Option.None(),
                new Color(snapshot.FillColor),
                new Color(snapshot.EdgeColor),
                Md5.FromByteArray(snapshot.Md5).UnwrapOrThrow(),
                snapshot.Sizes.Select(t => new ImageSize(
                    Name.FromString(t.Tag).UnwrapOrThrow(),
                    Resolution.FromScalar(t.Width, t.Height).UnwrapOrThrow(),
                    AspectRatio.FromString(t.AspectRatio).UnwrapOrThrow(),
                    t.CropStrategy != null ? Option.Some((CropStrategy)Enum.Parse(typeof(CropStrategy), t.CropStrategy, true)) : Option.None(),
                    ImageFormat.FromExtension(t.ImageFormat).UnwrapOrThrow()
                )).ToArray()
            ).UnwrapOrThrow();
        }

        public async Task<Result<Unit, Error>> Insert(Name name, IEnumerable<Image> images)
        {
            await _client.Images(name).InsertManyAsync(images.Select(i => ToSnapshot(i)));
            return Result.Unit();
        }

        public async Task<Result<Unit, Error>> Update(Name name, IEnumerable<Image> images)
        {
            foreach (var image in images)
                await _client.Images(name).ReplaceOneAsync(i => i.Id == image.Id.ToByteArray(), ToSnapshot(image));
            return Result.Unit();
        }

        public async Task<Result<Unit, Error>> Truncate(Name name)
        {
            await _client.Database.DropCollectionAsync(_client.ImagesCollectionName(name));
            return Result.Unit();
        }

        private ImageSnapshot ToSnapshot(Image meta)
        {
            return new ImageSnapshot
            {
                Id = meta.Id.ToByteArray(),
                RowVersion = meta.RowVersion,
                UploadedAt = meta.UploadedAt.ToDateTimeUtc(),
                Meta = meta.Meta.UnwrapOrDefault(),
                EdgeColor = meta.EdgeColor.ToScalar(),
                FillColor = meta.FillColor.ToScalar(),
                Md5 = meta.Md5.ToByteArray(),
                Sizes = meta.Sizes.Select(t => new ImageSizeSnapshot
                {
                    Tag = t.Tag.ToString(),
                    Width = t.Resolution.Width,
                    Height = t.Resolution.Height,
                    AspectRatio = t.AspectRatio.ToString(),
                    ImageFormat = t.ImageFormat.FileExtension,
                    CropStrategy = t.CropStrategy.Map(f => f.ToString()).UnwrapOrDefault()
                }).ToArray()
            };
        }
    }
}
