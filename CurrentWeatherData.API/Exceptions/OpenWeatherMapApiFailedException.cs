using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class OpenWeatherMapApiFailedException : BaseException
    {
        public OpenWeatherMapApiFailedException(string details, Exception innerException) 
            : base("Failed to retrieve data. The error message was \"" + details + "\"", innerException, HttpStatusCode.InternalServerError)
        {

        }
    }
}
