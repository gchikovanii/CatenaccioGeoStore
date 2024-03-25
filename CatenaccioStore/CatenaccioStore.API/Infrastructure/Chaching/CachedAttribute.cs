using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace CatenaccioStore.API.Infrastructure.Chaching
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveMinutes;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveMinutes = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var chacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var chachedResponse = await cacheService.GetCachedReponseAsync(chacheKey);
            if (!string.IsNullOrEmpty(chachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = chachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }
            var executedContext = await next(); //Moving to controller if in redis is empty
            if(executedContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.CacheResponseAsync(chacheKey,okObjectResult.Value,TimeSpan.FromMinutes(_timeToLiveMinutes));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key,value) in request.Query.OrderBy(i => i.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
