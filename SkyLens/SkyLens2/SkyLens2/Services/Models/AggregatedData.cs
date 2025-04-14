using System;

namespace SkyLens2.Services.Models
{
    public class AggregatedData
    {
        public float CloudCoverage { get; set; }
        public float WindSpeed { get; set; }
        public float Temperature { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public double SunAltitude { get; set; }
        public double SunAzimuth { get; set; }
        public string MoonPhase { get; set; }
        public string StageOfNight { get; set; }
    }
}