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
        /// Country Code validation
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static bool ValidateCountryCode(string country)
        {
            if (String.IsNullOrWhiteSpace(country))
                return false;
            else if (country.Length < 2)
                return false;

            return true;
        }

        /// <summary>
        /// City Name validation
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
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
