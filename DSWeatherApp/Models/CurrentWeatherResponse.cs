﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSWeatherApp.Models
{
    public class CurrentWeatherResponse
    {
        public List<Weather>? Weather{ get; set; }
        public Main? Main { get; set; }
        public string? Icon => Weather?.FirstOrDefault()?.icon;
        public double? Temperature => Main?.Temp;

        public string CityName { get; set; }
    }
}