using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class MissingApiKeyException : BaseException
    {
        public MissingApiKeyException() 
            : base("API Key Missing. Please provide API key via request header [X-API-KEY] OR query string [api-key]", HttpStatusCode.Unauthorized)
        {

        }
    }
}
