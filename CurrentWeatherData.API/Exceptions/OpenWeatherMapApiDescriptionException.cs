using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class OpenWeatherMapApiDescriptionException : BaseException
    {
        public OpenWeatherMapApiDescriptionException(string details, Exception innerException) 
            : base("Failed to read weather description. " + details, innerException, HttpStatusCode.InternalServerError)
        {

        }
    }
}
