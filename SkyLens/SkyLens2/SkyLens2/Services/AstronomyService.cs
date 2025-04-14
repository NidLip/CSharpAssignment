using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkyLens2.Services.Models;
using SkyLens2.Utilities;

namespace SkyLens2.Services
{
    public class AstronomyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AstronomyService(string? apiKey = null)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                EnvLoader.Load("apiKeys.env");
                apiKey = Environment.GetEnvironmentVariable("IpGeoAstronomyKey");
            }

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("IPGeolocation Astronomy API key is not set.");

            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<AstronomyData> GetAstronomyDataAsync(double lat, double lon)
        {
            string latStr = lat.ToString(CultureInfo.InvariantCulture);
            string lonStr = lon.ToString(CultureInfo.InvariantCulture);
            string url = $"https://api.ipgeolocation.io/astronomy?apiKey={_apiKey}&lat={latStr}&long={lonStr}";

            Console.WriteLine("Fetching Astronomy data from IPGeolocation:");
            Console.WriteLine(url);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception($"IPGeolocation API error: {response.StatusCode}\n{content}");
            }

            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("IPGeolocation API raw response:");
            Console.WriteLine(json);

            dynamic data = JsonConvert.DeserializeObject(json)
                ?? throw new Exception("Failed to parse IPGeolocation astronomy data.");

            DateTime sunrise = DateTime.Parse((string)data.sunrise);
            DateTime sunset = DateTime.Parse((string)data.sunset);
            double sunAltitude = double.Parse((string)data.sun_altitude, CultureInfo.InvariantCulture);
            double sunAzimuth = double.Parse((string)data.sun_azimuth, CultureInfo.InvariantCulture);
            string moonPhase = (string)data.moon_phase;

            return new AstronomyData
            {
                Sunrise = sunrise,
                Sunset = sunset,
                SunAltitude = sunAltitude,
                SunAzimuth = sunAzimuth,
                MoonPhase = moonPhase
            };
        }
    }
}