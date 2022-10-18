using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using CurrentWeatherData.API.Exceptions;
using CurrentWeatherData.API.Helpers;

namespace CurrentWeatherData.API.Authorization
{
    public class ApiKeyValidationHandler : AuthorizationHandler<ApiKeyValidationRequirement>
    {
        IHttpContextAccessor _httpContextAccessor = null;
        public ApiKeyValidationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                        ApiKeyValidationRequirement requirement)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            string apiKey = Common.GetApiKeyFromHttpContext(httpContext);

            if (apiKey == string.Empty)
            {
                throw new MissingApiKeyException();
            }
            else if (!requirement.IsApiKeyValid(apiKey))
            {
                throw new InvalidApiKeyException();
            }
            else
            {
                //requirement is met
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }

}
