using System.Collections.Generic;

namespace ApiVersioning.Controllers
{
    public class WeatherResponse
    {
        public string ApiVersion { get; set; }
        public IEnumerable<WeatherForecast> Forecasts { get; set; }
    }
}
