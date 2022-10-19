using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CurrentWeatherData.API.Services;
using Microsoft.AspNetCore.Authorization;
using CurrentWeatherData.API.Middlewares;
using CurrentWeatherData.API.Models;
using System.Net;

namespace CurrentWeatherData.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly CurrentWeatherDataService _currentWeatherDataService;

        public WeatherController(ILogger<WeatherController> logger, CurrentWeatherDataService currentWeatherDataService)
        {
            _logger = logger;
            _currentWeatherDataService = currentWeatherDataService;

        }

        [HttpGet]
        [Authorize(Policy = "ApiKeyValidRequirement")]
        [ApiKeyRateLimitDecorator()]
        async public Task<IActionResult> Get([FromQuery]WeatherRequest request)
        {
            string desc = await _currentWeatherDataService.GetCurrentWeatherDescription(request.Country, request.City);

            WeatherResponse response = new WeatherResponse()
            {
                Description = desc,
            };

            return Ok(response);
        }
    }
}
