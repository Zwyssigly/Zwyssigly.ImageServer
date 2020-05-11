using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Thumbnails;

namespace Zwyssigly.ImageServer.MongoDb
{
    internal class ThumbnailRepository : IThumbnailRepository
    {
        private readonly MongoDbClient _client;

        public ThumbnailRepository(MongoDbClient client)
        {
            _client = client;
        }

        public async Task<Result<Unit, Error>> Insert(Name gallery, IEnumerable<Thumbnail> thumbnails)
        {
            var snapshots = thumbnails.Select(t => new ThumbnailSnapshot
            {
                Id = new ThumbnailIdSnapshot
                {
                    ImageId = t.ThumbnailId.ImageId.ToByteArray(),
                    Tag = t.ThumbnailId.Tag.ToString()
                },
                Data = t.Data,
            });

            await _client.Thumbnails(gallery).InsertManyAsync(snapshots);
            return Result.Unit();
        }

        public async Task<Result<Thumbnail, Error>> Get(Name gallery, ThumbnailId id)
        {
            var cursor = await _client.Thumbnails(gallery).FindAsync(s => s.Id.ImageId == id.ImageId.ToByteArray() && s.Id.Tag == id.Tag.ToString()).ConfigureAwait(false);
            var snapshot = await cursor.ToListAsync().ConfigureAwait(false);

            return snapshot.Count == 1
                ? Result.Success<Thumbnail, Error>(new Thumbnail(
                    thumbnailId: new ThumbnailId(new Id(snapshot[0].Id.ImageId), Name.FromString(snapshot[0].Id.Tag).UnwrapOrThrow(), Option.None()),
                    data: snapshot[0].Data))
                : Result.Failure<Thumbnail, Error>(ErrorCode.NoSuchRecord);            
        }

        public async Task<Result<Unit, Error>> Delete(Name gallery, IEnumerable<ThumbnailId> ids)
        {
            var byteIds = ids.Select(id => new ThumbnailIdSnapshot { ImageId = id.ImageId.ToByteArray(), Tag = id.Tag.ToString() }).ToArray();

            var filter = Builders<ThumbnailSnapshot>.Filter.In(x => x.Id, byteIds);
            var response = await _client.Thumbnails(gallery).DeleteManyAsync(filter);

            return response.DeletedCount == byteIds.Length
                ? Result.Unit<Error>()
                : Result.Failure<Unit, Error>(ErrorCode.NoSuchRecord);
        }

        public async Task<Result<Unit, Error>> Truncate(Name gallery)
        {
            await _client.Database.DropCollectionAsync(_client.ThumbnailsCollectionName(gallery));
            return Result.Unit();
        }
    }
}
