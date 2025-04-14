using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkyLens2.Services.Models;
using SkyLens2.Utilities;

namespace SkyLens2.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiKey;

        // Parameterless constructor using EnvLoader
        public WeatherService()
        {
            // Load env file if the key is not already set
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OpenWeatherMap")))
            {
                EnvLoader.Load("apiKeys.env");
            }
            _apiKey = Environment.GetEnvironmentVariable("OpenWeatherMap");
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("OpenWeatherMap key is not set.");
            }
        }

        // Optionally, you can keep this overload for DI:
        public WeatherService(string apiKey) : this()
        {
            // Overwrite key if provided.
            if (!string.IsNullOrEmpty(apiKey))
            {
                _apiKey = apiKey;
            }
        }

        public async Task<WeatherData> GetWeatherDataAsync(double lat, double lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            // Debug: Write the raw JSON response to the console.
            Console.WriteLine("OpenWeatherMap API raw response:");
            Console.WriteLine(json);

            dynamic data = JsonConvert.DeserializeObject(json) 
                           ?? throw new Exception("Failed to deserialize JSON response.");            WeatherData weather = new WeatherData
            {
                CloudCoverage = (float)data.clouds.all,
                WindSpeed = (float)data.wind.speed,
                Temperature = (float)data.main.temp
            };

            return weather;
        }
    }
}