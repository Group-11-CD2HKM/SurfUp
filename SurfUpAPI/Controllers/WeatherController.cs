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

        public WeatherController(IHttpClientFactory client)
        {
            _client = client.CreateClient("OpenWeatherClient");
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery]string? cityName)
        {
            //Via et kald til Openweather.org, returnér 5dages vejrdata for en by

            HttpResponseMessage response = await _client.GetAsync($"data/2.5/forecast?q={cityName},DK&appid={_openWeatherAPIKey}");
            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<Root>();

                return Ok(weatherData);
            }

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("City not found");
            }

            return BadRequest("Invalid API key or an unexpected error. Test Openweather API directly for more details...");
        }
    }
}
