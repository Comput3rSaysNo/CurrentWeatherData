using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CurrentWeatherData.API.Services;
using Microsoft.AspNetCore.Authorization;
using CurrentWeatherData.API.Middlewares;

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
        async public Task<IActionResult> Get(string country, string city)
        {
            return Ok(await _currentWeatherDataService.GetCurrentWeatherDescription(country, city));
        }
    }
}
