using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class InvalidCountryCodeException : BaseException
    {
        public InvalidCountryCodeException() : base("Invalid Country Code.", HttpStatusCode.BadRequest)
        {
            
        }
    }
}
