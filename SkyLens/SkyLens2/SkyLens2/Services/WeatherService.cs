using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkyLens2.Services.Models;
using SkyLens2.Utilities;

namespace SkyLens2.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(string? apiKey = null)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                EnvLoader.Load("apiKeys.env");
                apiKey = Environment.GetEnvironmentVariable("OpenWeatherMap");
            }

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("OpenWeatherMap API key is not set.");

            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<WeatherData> GetWeatherDataAsync(double lat, double lon)
        {
            string latStr = lat.ToString(CultureInfo.InvariantCulture);
            string lonStr = lon.ToString(CultureInfo.InvariantCulture);
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latStr}&lon={lonStr}&appid={_apiKey}&units=metric";

            Console.WriteLine("Fetching Weather data from:");
            Console.WriteLine(url);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenWeatherMap API error: {response.StatusCode}\n{content}");
            }

            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("OpenWeatherMap API raw response:");
            Console.WriteLine(json);

            dynamic data = JsonConvert.DeserializeObject(json)
                ?? throw new Exception("Failed to parse weather data.");

            return new WeatherData
            {
                CloudCoverage = (float)data.clouds.all,
                WindSpeed = (float)data.wind.speed,
                Temperature = (float)data.main.temp
            };
        }
    }
}