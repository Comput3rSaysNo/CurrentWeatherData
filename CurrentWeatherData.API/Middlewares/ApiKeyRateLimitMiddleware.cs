using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using CurrentWeatherData.API.Exceptions;
using CurrentWeatherData.API.Helpers;
using System.Collections.Concurrent;

namespace CurrentWeatherData.API.Middlewares
{
    public class ApiKeyRateLimitMiddlware
    {
        
        private readonly RequestDelegate _next;

        private ApiKeyRateLimitMiddlwareOptions _options;

        ConcurrentDictionary<string, List<long>> apiCallTracker = new ConcurrentDictionary<string, List<long>>();

        public ApiKeyRateLimitMiddlware(RequestDelegate next, ApiKeyRateLimitMiddlwareOptions options)
        {
            _next = next;
            _options = options;
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
       
            // apply rate limit based on path
            if (!ApplyRateLimit(path, apiKey))
            {
                // apply global limit
                // only apply when path based rate limit is not applied
                ApplyRateLimit("*", apiKey);
            }   

            await _next(context);
        }

        /// <summary>
        /// apply api key rate limit for the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="apiKey"></param>
        /// <returns>returns TRUE if rate limit is applied</returns>
        /// <exception cref="ApiKeyRateLimitException"></exception>
        private bool ApplyRateLimit(string path, string apiKey)
        {
            string key = path + '_' + apiKey;

            ApiKeyRateLimitOption option;
            bool rateLimitRuleMatched = _options.RateLimit.TryGetValue(path, out option);

            if (rateLimitRuleMatched)
            {
                // now
                long nowUnixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                // when to expire the access records
                long expireUnixTimestamp = (nowUnixTimestamp - option.RateLimitInterval);

                // get list of access records that matches the path+api_key combo
                List<long> apiCallHistory = apiCallTracker.GetOrAdd(key, new List<long>());

                // remove expired records 
                apiCallHistory.RemoveAll(o => o <= expireUnixTimestamp);

                // check if the count is below the rate limit
                if (apiCallHistory.Count >= option.RateLimit)
                {
                    // deny access if rate limit is reached
                    throw new ApiKeyRateLimitException(String.Format("Details: [{2}] Api key is rate limited to {0} requests every {1} seconds", option.RateLimit, option.RateLimitInterval, path));
                }
                else
                {
                    // grant access and record the api call
                    apiCallHistory.Add(nowUnixTimestamp);
                }

            }

            // indicate if the rate limit was applied or not
            return rateLimitRuleMatched;
        }
    }
}
