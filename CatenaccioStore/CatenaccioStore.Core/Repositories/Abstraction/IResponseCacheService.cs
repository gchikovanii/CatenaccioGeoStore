namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string chacheKey, object response, TimeSpan timeToLive);

        Task<string> GetCachedReponseAsync(string chacheKey);

    }
}
