using Newtonsoft.Json;
using System.Net;

namespace CurrentWeatherData.API.Models
{
    public class WeatherResponse
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
