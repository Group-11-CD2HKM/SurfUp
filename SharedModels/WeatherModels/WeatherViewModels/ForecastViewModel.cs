using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.WeatherModels.WeatherViewModels
{
    public class ForecastViewModel
    {
        public string? Description { get; set; }
        public List<WeatherViewModel> WeatherViewModelList { get; set; } = new List<WeatherViewModel>();
    }
}
