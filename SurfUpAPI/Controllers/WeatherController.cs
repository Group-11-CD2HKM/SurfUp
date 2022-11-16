using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels.WeatherModels;
using static System.Net.WebRequestMethods;

namespace SurfUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _openWeatherAPIKey = "5a03b2f54d71377094b40207093cb606";

        public WeatherController(HttpClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery]string? cityName, [FromQuery]int forecastLength)
        {
            //Via et kald til Openweather.org, returnér vejrdata for 5 dage ud fra et bynavn

            HttpResponseMessage response = await _client.GetAsync($"/data/2.5/forecast?q={cityName ?? "Odense"},DK&appid={_openWeatherAPIKey}");
            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<Root>();
                return Ok(weatherData);
            }
            return BadRequest();
        }
    }
}
