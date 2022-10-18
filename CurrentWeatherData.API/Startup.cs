using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrentWeatherData.API.Services;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using CurrentWeatherData.API.Authorization;
using CurrentWeatherData.API.Middlewares;
using System.Numerics;
using System.Dynamic;

namespace CurrentWeatherData.API
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // enable httpcontextaccessor for our Authorization handler
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            // configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            });

            // configure API key handler
            services.AddAuthorization(options =>
            {
                string[] validApiKeys = _configuration.GetSection("CurrentWeatherData_Api:ApiKeys").Get<string[]>();

                options.AddPolicy("ApiKeyValidRequirement",
                    policy => policy.Requirements.Add(new ApiKeyValidationRequirement(validApiKeys)));
            });
            services.AddSingleton<IAuthorizationHandler, ApiKeyValidationHandler>();

            // configure weather data service
            services.AddSingleton<CurrentWeatherDataService>(x =>
            {
                HttpClient client = new HttpClient();

                string baseUri = _configuration["OpenWeatherMap_Api:BaseUri"];
                string apiKey = _configuration["OpenWeatherMap_Api:ApiKey"];

                return new CurrentWeatherDataService(client, baseUri, apiKey);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            dynamic[] result = _configuration.GetSection("CurrentWeatherData_Api:RateLimit").Get<ExpandoObject[]>();

            // configure rate limiting middleware
            ApiKeyRateLimitMiddlwareOptions apiKeyRateLimitMiddlwareOptions = new ApiKeyRateLimitMiddlwareOptions
            {
                RateLimit = result.ToDictionary(
                    o => (string)o.Path,
                    o => new ApiKeyRateLimitOption(int.Parse(o.RateLimit), int.Parse(o.RateLimitInterval))
                )
            };
            app.UseMiddleware<ApiKeyRateLimitMiddlware>(apiKeyRateLimitMiddlwareOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
