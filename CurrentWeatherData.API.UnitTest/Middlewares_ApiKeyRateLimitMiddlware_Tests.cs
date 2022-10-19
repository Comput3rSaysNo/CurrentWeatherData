using CurrentWeatherData.API.Exceptions;
using CurrentWeatherData.API.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrentWeatherData.API.UnitTest
{
    [TestClass]
    public class Middlewares_ApiKeyRateLimitMiddlware_Tests
    {
        [TestMethod]
        public void ShouldDenyAccessAfterFiveReuqests_PathRateLimit()
        {
            ApiKeyRateLimitMiddlware middleware = prepareMiddleware();
            HttpContext context = prepareHttpContext("/allow_5_per_30_secs");

            // perform test
            RunMiddleware(middleware, context, 5, 20);
        }

        [TestMethod]
        public void ShouldDenyAccessAfterTenReuqests_GlobalRateLimit()
        {
            ApiKeyRateLimitMiddlware middleware = prepareMiddleware();
            HttpContext context = prepareHttpContext("/some_path_with_no_decorator_global_rate_limit_apply");

            // perform test
            RunMiddleware(middleware, context, 10, 20);
        }

        [TestMethod]
        public void ShouldDenyAccessAfterFiveReuqests_ThenRegainAccessAfterTenSeconds_PathRateLimit()
        {
            ApiKeyRateLimitMiddlware middleware = prepareMiddleware();
            HttpContext context = prepareHttpContext("/allow_5_per_10_secs");
            
            // perform test
            RunMiddleware(middleware, context, 5, 20);

            Task.Delay(10000).Wait();

            RunMiddleware(middleware, context, 5, 20);
        }

        [ApiKeyRateLimitDecorator()]
        public void DummyControllerMethodWithRateLimit()
        {

        }

        public HttpContext prepareHttpContext(string requestPath)
        {
            string apiKey = "7f036c0d-72fd-45c7-b212-5d4a7feb3c0d";

            HttpContext context = new DefaultHttpContext();
            context.Request.Headers.Add("X-API-KEY", apiKey);
            context.Request.Path = new PathString(requestPath);
            context.SetEndpoint(new Endpoint(
                new RequestDelegate(async (innerContext) =>
                {
                    await Task.FromResult(0);
                }),
                new EndpointMetadataCollection(new object[] {
                    new ControllerActionDescriptor()
                    {
                        MethodInfo = typeof(Middlewares_ApiKeyRateLimitMiddlware_Tests).GetMethod("DummyControllerMethodWithRateLimit")
                    },
                    new ApiKeyRateLimitDecorator()
                }),
                "weather controller"
            ));

            return context;
        }

        public static RequestDelegate prepareRequestDelegate()
        {
            HttpContext context = new DefaultHttpContext();
            var requestDelegate = new RequestDelegate(async (innerContext) =>
            {
                await Task.FromResult(0);
            });

            return requestDelegate;
        }

        public static ApiKeyRateLimitMiddlware prepareMiddleware()
        {
            RequestDelegate d = prepareRequestDelegate();

            ApiKeyRateLimitMiddlwareOptions options = new ApiKeyRateLimitMiddlwareOptions();
            options.RateLimit.Add("*", new ApiKeyRateLimitOption(10, 30));
            options.RateLimit.Add("/allow_5_per_30_secs", new ApiKeyRateLimitOption(5, 30));
            options.RateLimit.Add("/allow_5_per_10_secs", new ApiKeyRateLimitOption(5, 10));

            MemoryCache cache = new MemoryCache(new MemoryCacheOptions
            {
            });

            var middleware = new ApiKeyRateLimitMiddlware(d, options, cache);

            return middleware;
        }

        public void RunMiddleware(ApiKeyRateLimitMiddlware middleware, HttpContext context, int expected, int limit)
        {
            int counter = 0;
            try
            {
                while (counter < limit)
                {
                    middleware.InvokeAsync(context).Wait();
                    counter++;
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is ApiKeyRateLimitException)
                    Assert.AreEqual(expected, counter);
                else
                    Assert.Fail(ex.ToString());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }

            Assert.AreEqual(expected, counter);
        }
    }
}
