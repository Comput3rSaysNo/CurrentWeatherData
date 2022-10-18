using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class ApiKeyRateLimitException : BaseException
    {
        public ApiKeyRateLimitException(string message) 
            : base("Api key rate limit reached. " + message, HttpStatusCode.TooManyRequests)
        {

        }
    }
}
