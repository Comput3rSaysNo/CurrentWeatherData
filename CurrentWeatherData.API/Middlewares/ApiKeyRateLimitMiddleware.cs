using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using CurrentWeatherData.API.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using CurrentWeatherData.API.Helpers;

namespace CurrentWeatherData.API.Middlewares
{
    public class ApiKeyRateLimitMiddlware
    {
        private readonly IMemoryCache _memoryCache;
        private readonly RequestDelegate _next;

        private ApiKeyRateLimitMiddlwareOptions _options;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public ApiKeyRateLimitMiddlware(RequestDelegate next, ApiKeyRateLimitMiddlwareOptions options, IMemoryCache memoryCache)
        {
            _next = next;

            _options = options;

            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (controllerActionDescriptor is null)
            {
                await _next(context);
                return;
            }

            var apiDecorator = (ApiKeyRateLimitDecorator)controllerActionDescriptor.MethodInfo
                            .GetCustomAttributes(true)
                            .SingleOrDefault(w => w.GetType() == typeof(ApiKeyRateLimitDecorator));

            if (apiDecorator is null)
            {
                await _next(context);
                return;
            }

            string apiKey = Common.GetApiKeyFromHttpContext(context);
            string path = context.Request.Path;
            string key = GetMemcacheKey(path, apiKey);

            // apply rate limit
            ApiKeyRateLimitOption option;
            if (_options.RateLimit.TryGetValue(path, out option) || _options.RateLimit.TryGetValue("*", out option))
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    int apiCallsWithinLastInterval = await _memoryCache.GetOrCreateAsync(key, cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(option.RateLimitInterval);
                        return Task.FromResult(0);
                    });

                    if (apiCallsWithinLastInterval >= option.RateLimit)
                    {
                        throw new ApiKeyRateLimitException(String.Format("Details: [{2}] Api key is rate limited to {0} requests every {1} seconds", option.RateLimit, option.RateLimitInterval, path));
                    }
                    else
                    {
                        apiCallsWithinLastInterval++;

                        _memoryCache.Set(key, apiCallsWithinLastInterval, new MemoryCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(option.RateLimitInterval)
                        });
                    }
                }
                finally
                {
                    //release the semaphore
                    semaphoreSlim.Release();
                }
            }
          
            await _next(context);
        }


        private static string GetMemcacheKey(string path, string apiKey)
        {
            
            var keys = new List<string>
            {
                path
            };

            keys.Add(apiKey);

            return string.Join('_', keys);
        }

    }
}
