using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class DailyWeatherResponse
    {
        [JsonProperty("list")]
        public List<DailyWeather>? List { get; set; }
    }
}
