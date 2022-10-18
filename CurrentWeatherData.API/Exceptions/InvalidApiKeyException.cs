using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class InvalidApiKeyException : BaseException
    {
        public InvalidApiKeyException() : base("Invalid API Key.", HttpStatusCode.Unauthorized)
        {
            
        }
    }
}
