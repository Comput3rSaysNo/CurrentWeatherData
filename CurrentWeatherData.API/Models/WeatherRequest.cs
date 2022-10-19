using Microsoft.AspNetCore.Mvc;

namespace CurrentWeatherData.API.Models
{
    public class WeatherRequest
    {
        [FromQuery]
        public string Country { get; set; }

        [FromQuery]
        public string City { get; set; }

    }
}
