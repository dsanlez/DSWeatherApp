using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class HourlyWeather
    {
        public string? Hour { get; set; }
        public string? Icon { get; set; }
        public string? Temperature { get; set; }

        [JsonProperty("main")]
        public Main? Main { get; set; }

        [JsonProperty("weather")]
        public List<Weather>? Weather { get; set; }

        [JsonProperty("dt_txt")]
        public string? DtTxt { get; set; }

        [JsonProperty("wind")]
        public Wind? Wind { get; set; }

        [JsonProperty("rain")]
        public Rain? Rain { get; set; }

        public string? CityName { get; set; }


    }
}
