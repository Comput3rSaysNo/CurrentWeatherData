using System;

namespace CurrentWeatherData.API.Middlewares
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiKeyRateLimitDecorator : Attribute
    {
    }
}
