using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prometeo.Planner.Console.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Geo
{
    public class OpenWeatherService
    {

        public CurrentWeatherConditionData QueryConditions(double lat, double lon)
        {
            var weatherapi = ConfigurationManager.AppSettings["weatherProviderUrl"].ToString();
            var currentWeather = RESTTools.Get(string.Format(weatherapi, lat, lon));

            if (string.IsNullOrEmpty(currentWeather))
                return new CurrentWeatherConditionData();

            var weatherData = JsonConvert.DeserializeObject<CurrentWeatherConditionData>(currentWeather);

            return weatherData;
        }
    }

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public double deg { get; set; }
        public string as_string { get => string.Format("{0:n0} mph DIR {1}°", speed, deg); }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public double message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class CurrentWeatherConditionData
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }
}
