using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class DailyForecast
    {
        public long dt { get; set; }
        public Temp temp { get; set; }
        public List<Weather> weather { get; set; }
    }
}
