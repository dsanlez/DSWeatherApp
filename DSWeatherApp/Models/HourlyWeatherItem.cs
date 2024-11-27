using Newtonsoft.Json;

namespace DSWeatherApp.Models
{
    public class HourlyWeatherItem
    {
        [JsonProperty("main")]
        public Main? Main { get; set; }

        [JsonProperty("weather")]
        public List<Weather>? Weather { get; set; }

        [JsonProperty("dt_txt")]
        public string? DtTxt { get; set; }
    }
}
