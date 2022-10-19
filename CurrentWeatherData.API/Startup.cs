using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using CurrentWeatherData.API.Services;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using CurrentWeatherData.API.Authorization;
using CurrentWeatherData.API.Middlewares;
using System.Dynamic;
using Microsoft.AspNetCore.Diagnostics;
using CurrentWeatherData.API.Exceptions;
using System.Text.Json;

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
            services.AddSingleton(x =>
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
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
        
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                Exception exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;

                HttpStatusCode errorCode = HttpStatusCode.InternalServerError;

                if (exception is BaseException)
                    errorCode = ((BaseException)exception).HttpErrorCode;

                dynamic response = new ExpandoObject();

                response.code = (int)errorCode;
                response.message = exception.Message;

                // only output stacktrace in dev environment
                if (env.IsDevelopment())
                    response.stacktrace = exception.ToString();

                context.Response.StatusCode = response.code;

                string jsonResponse = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(jsonResponse);
            }));
    
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            // configure rate limiting middleware
            dynamic[] result = _configuration.GetSection("CurrentWeatherData_Api:RateLimit").Get<ExpandoObject[]>();
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
