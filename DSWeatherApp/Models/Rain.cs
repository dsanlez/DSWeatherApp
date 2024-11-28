using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class Rain
    {
        [JsonProperty("1h")]
        public double _1h { get; set; }
    }
}
