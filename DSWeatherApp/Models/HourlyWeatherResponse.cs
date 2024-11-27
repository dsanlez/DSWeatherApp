using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class HourlyWeatherResponse
    {
        [JsonProperty("list")]
        public List<HourlyWeatherItem>? List { get; set; }
    }
}
