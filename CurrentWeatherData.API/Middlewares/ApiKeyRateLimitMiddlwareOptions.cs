using System.Collections.Generic;

namespace CurrentWeatherData.API.Middlewares
{
    public class ApiKeyRateLimitMiddlwareOptions
    {
        public Dictionary<string, ApiKeyRateLimitOption> RateLimit = new Dictionary<string, ApiKeyRateLimitOption>();
    }
}
