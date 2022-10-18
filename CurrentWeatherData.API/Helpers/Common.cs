using Microsoft.AspNetCore.Http;

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
    }
}
