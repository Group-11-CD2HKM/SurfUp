using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.WeatherModels.WeatherViewModels
{
    public class WeatherViewModel
    {
        public string Description { get; set; }

        private double tempCelcius;

        public double TempCelcius
        {
            get { return (int)tempCelcius; }
            set { tempCelcius = (value - 273.15); ; } //kelvin til celcius
        }

        public string? TimeOfDay { get; set; }

        public string? Icon { get; set; }
    }
}
