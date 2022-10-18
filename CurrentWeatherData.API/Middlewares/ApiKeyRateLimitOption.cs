namespace CurrentWeatherData.API.Middlewares
{
    public class ApiKeyRateLimitOption
    {
        public int RateLimit { get; set; }
        public int RateLimitInterval { get; set; }

        public ApiKeyRateLimitOption(int rateLimit, int rateLimitInterval)
        {
            RateLimit = rateLimit;
            RateLimitInterval = rateLimitInterval;
        }
    }
}
