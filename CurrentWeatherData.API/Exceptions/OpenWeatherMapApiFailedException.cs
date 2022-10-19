using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class OpenWeatherMapApiFailedException : BaseException
    {
        public OpenWeatherMapApiFailedException(HttpStatusCode status, string message, Exception innerException) 
            : base("Error loading current weather info. Details: " + message, innerException, status)
        {

        }

    }
}
