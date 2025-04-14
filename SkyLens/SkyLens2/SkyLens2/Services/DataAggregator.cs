using System;
using System.Threading.Tasks;
using SkyLens2.Services.Models;

namespace SkyLens2.Services
{
    public class DataAggregator
    {
        private readonly WeatherService _weatherService;
        private readonly AstronomyService _astronomyService;

        // Use parameterless constructors—services themselves load their API keys.
        public DataAggregator()
        {
            _weatherService = new WeatherService();
            _astronomyService = new AstronomyService();
        }

        public async Task<AggregatedData> GetAggregatedDataAsync(double lat, double lon)
        {
            // Call API endpoints concurrently.
            Task<WeatherData> weatherTask = _weatherService.GetWeatherDataAsync(lat, lon);
            Task<AstronomyData> astronomyTask = _astronomyService.GetAstronomyDataAsync(lat, lon);

            await Task.WhenAll(weatherTask, astronomyTask);

            WeatherData weather = weatherTask.Result;
            AstronomyData astro = astronomyTask.Result;
            DateTime now = DateTime.UtcNow;

            AggregatedData aggregated = new AggregatedData
            {
                CloudCoverage = weather.CloudCoverage,
                WindSpeed = weather.WindSpeed,
                Temperature = weather.Temperature,
                Sunrise = astro.Sunrise,
                Sunset = astro.Sunset,
                SunAltitude = astro.SunAltitude,
                SunAzimuth = astro.SunAzimuth,
                MoonPhase = astro.MoonPhase,
                StageOfNight = DetermineStageOfNight(astro.Sunrise, astro.Sunset, now)
            };

            return aggregated;
        }

        // Simple logic to determine the stage of the night.
        private string DetermineStageOfNight(DateTime sunrise, DateTime sunset, DateTime now)
        {
            if (now < sunrise)
                return "Before Dawn";
            if (now >= sunrise && now < sunset)
                return "Day";

            var minutesSinceSunset = (now - sunset).TotalMinutes;
            if (minutesSinceSunset <= 60)
                return "Civil Twilight";
            if (minutesSinceSunset <= 90)
                return "Nautical Twilight";
            if (minutesSinceSunset <= 120)
                return "Astronomical Twilight";
            return "Night";
        }
    }
}