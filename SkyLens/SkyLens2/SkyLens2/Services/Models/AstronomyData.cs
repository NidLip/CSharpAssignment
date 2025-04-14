using System;

namespace SkyLens2.Services.Models
{
    public class AstronomyData
    {
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public double SunAltitude { get; set; }
        public double SunAzimuth { get; set; }
        public string MoonPhase { get; set; }
    }
}