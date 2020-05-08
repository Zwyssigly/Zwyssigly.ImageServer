using System.Net.Http;
using System.Threading.Tasks;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Core
{
    public interface IHttpClientWrapper
    {
        public Task<Result<byte[], Error>> Get(string url);
    }

    public class NullHttpClientWrapper : IHttpClientWrapper
    {
        public Task<Result<byte[], Error>> Get(string url)
            => Task.FromResult(Result.Failure<byte[], Error>(ErrorCode.NotSupported));
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public async Task<Result<byte[], Error>> Get(string url)
        {
            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return Result.Success(await response.Content.ReadAsByteArrayAsync());

            return Result.Failure<byte[], Error>(ErrorCode.NoSuchRecord);
        }
    }
}
