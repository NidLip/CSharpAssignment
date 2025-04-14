using System;
using System.Threading.Tasks;
using SkyLens2.Services.Models;

namespace SkyLens2.Services
{
    public class DataAggregator
    {
        private readonly WeatherService _weatherService;
        private readonly AstronomyService _astronomyService;
        private readonly LocationService _locationService;

        public DataAggregator()
        {
            _weatherService = new WeatherService();
            _astronomyService = new AstronomyService();
            _locationService = new LocationService();
        }

        public async Task<AggregatedData> GetAggregatedDataAsync()
        {
            var (lat, lon, city, country, offsetMinutes) = await _locationService.GetUserLocationInfoAsync();

            Task<WeatherData> weatherTask = _weatherService.GetWeatherDataAsync(lat, lon);
            Task<AstronomyData> astronomyTask = _astronomyService.GetAstronomyDataAsync(lat, lon);

            await Task.WhenAll(weatherTask, astronomyTask);

            var weather = weatherTask.Result;
            var astro = astronomyTask.Result;
            var localNow = DateTime.UtcNow.AddMinutes(offsetMinutes);

            return new AggregatedData
            {
                CloudCoverage = weather.CloudCoverage,
                WindSpeed = weather.WindSpeed,
                Temperature = weather.Temperature,
                Sunrise = astro.Sunrise,
                Sunset = astro.Sunset,
                SunAltitude = astro.SunAltitude,
                SunAzimuth = astro.SunAzimuth,
                MoonPhase = FormatMoonPhase(astro.MoonPhase),
                StageOfNight = DetermineStageOfNight(astro.SunAltitude),
                LocationName = $"{city}, {country}"
            };
        }

        private string FormatMoonPhase(string rawPhase)
        {
            if (string.IsNullOrWhiteSpace(rawPhase))
                return "Unknown";

            var words = rawPhase.ToLower().Replace('_', ' ').Split(' ');
            for (int i = 0; i < words.Length; i++)
                words[i] = char.ToUpper(words[i][0]) + words[i][1..];

            return string.Join(" ", words);
        }

        private string DetermineStageOfNight(double sunAltitude)
        {
            if (sunAltitude > 0)
                return "Day";
            else if (sunAltitude > -6)
                return "Civil Twilight";
            else if (sunAltitude > -12)
                return "Nautical Twilight";
            else if (sunAltitude > -18)
                return "Astronomical Twilight";
            else
                return "Night";
        }
    }
}