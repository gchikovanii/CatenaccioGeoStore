using CatenaccioStore.Core.Repositories.Abstraction;
using StackExchange.Redis;
using System.Text.Json;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CacheResponseAsync(string chacheKey, object response, TimeSpan timeToLive)
        {
            if(response == null)
                return;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var serilzedResponse = JsonSerializer.Serialize(response, options);
            await _database.StringSetAsync(chacheKey, serilzedResponse, timeToLive);
        }

        public async Task<string> GetCachedReponseAsync(string chacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(chacheKey);
            if (cachedResponse.IsNullOrEmpty)
                return null;
            return cachedResponse;
        }
    }
}
