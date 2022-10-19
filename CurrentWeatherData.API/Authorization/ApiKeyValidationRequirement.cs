using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace CurrentWeatherData.API.Authorization
{
    public class ApiKeyValidationRequirement : IAuthorizationRequirement
    {
        // Store the list of valid api keys
        public HashSet<string> ValidApiKeys { get; private set; }

        public ApiKeyValidationRequirement(string[] validApiKeys)
        {
            ValidApiKeys = validApiKeys.ToHashSet();
        }
    }
}
