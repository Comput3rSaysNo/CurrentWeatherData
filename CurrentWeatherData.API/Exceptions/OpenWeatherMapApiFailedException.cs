using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class OpenWeatherMapApiFailedException : BaseException
    {
        public OpenWeatherMapApiFailedException(Exception innerException) 
            : base("Error loading current weather info. Api error.", innerException, HttpStatusCode.InternalServerError)
        {

        }

    }
}
