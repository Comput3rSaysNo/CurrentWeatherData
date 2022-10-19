using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrentWeatherData.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CurrentWeatherData.API.Exceptions;
using System;
using System.Linq;

namespace CurrentWeatherData.API.UnitTest
{
    [TestClass]
    public class Authorization_ApiKeyValidationHandler_Tests : ApiKeyValidationHandler
    {
        public static HttpContextAccessor arrangeHttpContextAccessor()
        {
            return new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        public static ApiKeyValidationRequirement arrangeApiKeyValidationRequirement()
        {
            var requirement = new ApiKeyValidationRequirement(new string[] {
                    "cdc73f3b-0af8-4575-98ed-ad334240e25a",
                    "8d1caf6f-0f5c-4819-ba6b-8ed92e3f7140"
                });

            return requirement;
        }

        public static AuthorizationHandlerContext prepareAuthorizationHandlerContext()
        {
            var requirement = arrangeApiKeyValidationRequirement();

            var requirements = new[] { requirement };
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] { },
                            "Basic")
                        );
            object resource = null;

            var authorizationHandlerContext = new AuthorizationHandlerContext(requirements, user, resource);

            return authorizationHandlerContext;
        }

        public Authorization_ApiKeyValidationHandler_Tests(): base (Authorization_ApiKeyValidationHandler_Tests.arrangeHttpContextAccessor())
        {
        }

        [TestMethod]
        public void ShouldThrowMissingApiKeyExceptionWhenApiKeyIsNotPresent()
        {
            //Arrange
            base._httpContextAccessor = arrangeHttpContextAccessor();
            var authorizationHandlerContext = prepareAuthorizationHandlerContext();
            
            try { 
                HandleRequirementAsync(authorizationHandlerContext
                    , (ApiKeyValidationRequirement)authorizationHandlerContext.Requirements.First()).Wait();

                Assert.Fail();
            }
            catch (MissingApiKeyException ex)
            {
                // success
            }
        }

        [TestMethod]
        public void ShouldThrowInvalidApiKeyExceptionWhenApiKeyIsNotAllowedRequestHeader()
        {
            //Arrange
            base._httpContextAccessor = arrangeHttpContextAccessor();
            base._httpContextAccessor.HttpContext.Request.Headers.Add("X-API-KEY", "0569db4e-ccbb-4e20-8f69-95e67476327a");

            var authorizationHandlerContext = prepareAuthorizationHandlerContext();

            try
            {
                HandleRequirementAsync(authorizationHandlerContext
                    , (ApiKeyValidationRequirement)authorizationHandlerContext.Requirements.First()).Wait();

                Assert.Fail();
            }
            catch (InvalidApiKeyException ex)
            {
                // success
            }
        }

        [TestMethod]
        public void ShouldThrowInvalidApiKeyExceptionWhenApiKeyIsNotAllowedQueryString()
        {
            //Arrange
            base._httpContextAccessor = arrangeHttpContextAccessor();
            base._httpContextAccessor.HttpContext.Request.QueryString = new QueryString("?API-KEY=0569db4e-ccbb-4e20-8f69-95e67476327a");

            var authorizationHandlerContext = prepareAuthorizationHandlerContext();

            try
            {
                HandleRequirementAsync(authorizationHandlerContext
                    , (ApiKeyValidationRequirement)authorizationHandlerContext.Requirements.First()).Wait();

                Assert.Fail();
            }
            catch (InvalidApiKeyException ex)
            {
                // success
            }
        }

        [TestMethod]
        public void ShouldAllowApiKeyInRequestHeader()
        {
            base._httpContextAccessor = arrangeHttpContextAccessor();
            base._httpContextAccessor.HttpContext.Request.Headers.Add("X-API-KEY", "cdc73f3b-0af8-4575-98ed-ad334240e25a");

            var authorizationHandlerContext = prepareAuthorizationHandlerContext();

            HandleRequirementAsync(authorizationHandlerContext
                    , (ApiKeyValidationRequirement)authorizationHandlerContext.Requirements.First()).Wait();

            Assert.IsTrue(authorizationHandlerContext.HasSucceeded);
        }

        [TestMethod]
        public void ShouldAllowApiKeyInQueryString()
        {
            base._httpContextAccessor = arrangeHttpContextAccessor();
            base._httpContextAccessor.HttpContext.Request.QueryString = new QueryString("?API-KEY=cdc73f3b-0af8-4575-98ed-ad334240e25a");
            //("API-KEY"]= "API-KEY=cdc73f3b-0af8-4575-98ed-ad334240e25a";

            var authorizationHandlerContext = prepareAuthorizationHandlerContext();

            HandleRequirementAsync(authorizationHandlerContext
                    , (ApiKeyValidationRequirement)authorizationHandlerContext.Requirements.First()).Wait();

            Assert.IsTrue(authorizationHandlerContext.HasSucceeded);
        }
    }
}
