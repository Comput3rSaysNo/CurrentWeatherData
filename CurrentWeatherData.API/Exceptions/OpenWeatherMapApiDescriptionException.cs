using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class OpenWeatherMapApiDescriptionException : BaseException
    {
        public OpenWeatherMapApiDescriptionException(Exception innerException) 
            : base("Error loading current weather info. Failed to read description.", innerException, HttpStatusCode.InternalServerError)
        {

        }
    }
}
