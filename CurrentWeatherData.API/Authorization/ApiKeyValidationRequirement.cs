using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace CurrentWeatherData.API.Authorization
{
    public class ApiKeyValidationRequirement : IAuthorizationRequirement
    {
        // Store the list of valid api keys
        private HashSet<string> _validApiKeys;

        public ApiKeyValidationRequirement(string[] validApiKeys)
        {
            _validApiKeys = validApiKeys.ToHashSet();
        }

        /// <summary>
        /// Checks if the api key is valid
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public bool IsApiKeyValid(string apiKey)
        {
            return _validApiKeys.Contains(apiKey);
        }
    }
}
