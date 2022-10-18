using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class InvalidCityNameException : BaseException
    {
        public InvalidCityNameException() : base("Invalid City Name.", HttpStatusCode.BadRequest)
        {
            
        }
    }
}
