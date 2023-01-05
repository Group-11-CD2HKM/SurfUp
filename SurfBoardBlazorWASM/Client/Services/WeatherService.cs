using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedModels.WeatherModels;
using SharedModels.WeatherModels.WeatherViewModels;
using System.Linq;
using System.Net.Http.Json;

namespace SurfBoardBlazorWASM.Client.Services
{
    public class WeatherService
    {
        private readonly HttpClient _client;
        public WeatherService(IHttpClientFactory client)
        {
            _client = client.CreateClient("SurfBoardBlazorWASM.PublicServerAPI");
        }

        public async Task<List<ForecastViewModel>> GetWeather(string? cityName)
        {
            //Hent vejrdata fra SurfUpAPI
            //Sortér weatherData således forecasts indeholder lister der hver især repræsenterer en enkelt dags vejrudsigt time for time

            List<ForecastViewModel> forecasts = new List<ForecastViewModel>();
            List<int> datesFound = new List<int>();
            Root weatherData;
            try
            {
                weatherData = await _client.GetFromJsonAsync<Root>($"api/weather?cityname={cityName}");
            } catch(Exception ex) 
            {
                return forecasts;
            }
            foreach (var weather in weatherData.list)
            {
                if (!datesFound.Contains(DateTime.Parse(weather.dt_txt).Day))
                {
                    datesFound.Add(DateTime.Parse(weather.dt_txt).Day);
                }
            }
            foreach (var weather in weatherData.list)
            {
                ForecastViewModel fvm = new ForecastViewModel();
                if (datesFound.Contains(DateTime.Parse(weather.dt_txt).Day))
                {
                    fvm.Description = $"{DateTime.Parse(weather.dt_txt).DayOfWeek.ToString()} {DateTime.Parse(weather.dt_txt).Date.ToShortDateString()}";
                    foreach (var weatherWithMatchingDate in weatherData.list.Where(w => DateTime.Parse(w.dt_txt).Day == DateTime.Parse(weather.dt_txt).Day))
                    {
                        fvm.WeatherViewModelList.Add(new WeatherViewModel
                        { 
                            Description = weatherWithMatchingDate.weather[0].description,
                            TempCelcius = weatherWithMatchingDate.main.temp,
                            TimeOfDay = DateTime.Parse(weatherWithMatchingDate.dt_txt).ToShortTimeString(),
                            Icon = $"http://openweathermap.org/img/wn/{weatherWithMatchingDate.weather[0].icon}@2x.png"
                        });
                    }
                    forecasts.Add(fvm);
                    datesFound.Remove(DateTime.Parse(weather.dt_txt).Day);
                }
            }

            return forecasts;
        }

        public async Task<bool> CheckForValidCityName(string? cityName)
        {
            //Undersøg om bruger inputtet for by er et gyldigt bynavn i Danmark

            HttpResponseMessage response = await _client.GetAsync($"api/weather?cityname={cityName}");         
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            return true;
        }
    }
}
