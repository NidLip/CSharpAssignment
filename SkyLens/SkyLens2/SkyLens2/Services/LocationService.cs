using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkyLens2.Utilities;

namespace SkyLens2.Services
{
    public class LocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public LocationService()
        {
            EnvLoader.Load("apiKeys.env");
            _apiKey = Environment.GetEnvironmentVariable("IpGeoKey")
                      ?? throw new Exception("IP Geolocation key not found.");
            _httpClient = new HttpClient();
        }

        public async Task<(double Latitude, double Longitude, string City, string Country, int TimezoneOffsetMinutes)> GetUserLocationInfoAsync()
        {
            string url = $"https://api.ipgeolocation.io/ipgeo?apiKey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"IP Geolocation failed: {response.StatusCode}\n{error}");
            }

            string json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json)
                           ?? throw new Exception("Failed to parse location data.");

            double lat = double.Parse(((string)data.latitude).Trim(), CultureInfo.InvariantCulture);
            double lon = double.Parse(((string)data.longitude).Trim(), CultureInfo.InvariantCulture);
            string city = data.city;
            string country = data.country_name;
            
            int offsetMinutes = (int)(data.time_zone.offset) * 60;

            Console.WriteLine($" Location: {city}, {country} | Offset: {offsetMinutes} mins");

            return (lat, lon, city, country, offsetMinutes);
        }
    }
}