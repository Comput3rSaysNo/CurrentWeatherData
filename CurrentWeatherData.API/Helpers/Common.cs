using CurrentWeatherData.API.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;

namespace CurrentWeatherData.API.Helpers
{
    public static class Common
    {

        /// <summary>
        /// get api key from request header or from query string
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>returns api key, empty if not found</returns>
        public static string GetApiKeyFromHttpContext(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("X-API-KEY"))
                return httpContext.Request.Headers["X-API-KEY"];
            else if (httpContext.Request.Query.ContainsKey("API-KEY"))
                return httpContext.Request.Query["API-KEY"];
            else
                return string.Empty;
        }

        /// <summary>
        /// Checks the Country Code
        /// </summary>
        /// <param name="country"></param>
        /// <exception cref="InvalidCountryCodeException"></exception>
        public static bool ValidateCountryCode(string country)
        {
            try
            {
                RegionInfo info = new RegionInfo(country);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the city name
        /// </summary>
        /// <param name="city"></param>
        /// <exception cref="InvalidCityNameException"></exception>
        public static bool ValidateCityName(string city)
        {
            if (String.IsNullOrWhiteSpace(city))
                return false;
            else if (city.Length <= 2)
                return false;

            return true;
        }
    }
}
