using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class CitySearchResponse
    {
        [JsonProperty("list")]
        public List<City>? CityList { get; set; }
    }
}
