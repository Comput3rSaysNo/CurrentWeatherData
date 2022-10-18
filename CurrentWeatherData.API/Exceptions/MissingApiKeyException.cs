using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class MissingApiKeyException : BaseException
    {
        public MissingApiKeyException() 
            : base("Api key missing. Include Api key in Request Header [X-API-KEY] or Query String ?api-key={value}", HttpStatusCode.Unauthorized)
        {

        }
    }
}
