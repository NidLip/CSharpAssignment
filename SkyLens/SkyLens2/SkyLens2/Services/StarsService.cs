using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SkyLens2.Models;

namespace SkyLens2.Services
{
    public class StarService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public StarService()
        {
            _apiKey = Environment.GetEnvironmentVariable("StarsApiKey");
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("[ERROR] StarsApiKey not found in environment.");
                throw new Exception("Stars API key is not set.");
            }

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        }

        public async Task<List<Star>> GetStarsAsync()
        {
            var allStars = new List<Star>();
            int offset = 0;
            const int limit = 30;

            while (true)
            {
                string url = $"https://api.api-ninjas.com/v1/stars?max_apparent_magnitude=6&offset={offset}";

                try
                {
                    var response = await _httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[ERROR] API call failed with status code: {response.StatusCode}");
                        break;
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Fetched stars batch offset {offset}:\n{json}");

                    var rawStars = JsonSerializer.Deserialize<List<RawStar>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (rawStars == null || rawStars.Count == 0)
                        break;

                    foreach (var raw in rawStars)
                    {
                        allStars.Add(new Star
                        {
                            Name = raw.Name ?? "",
                            Constellation = raw.Constellation ?? "",
                            RightAscension = raw.RightAscension ?? "",
                            Declination = raw.Declination ?? "",
                            ApparentMagnitude = ParseDouble(raw.ApparentMagnitude),
                            AbsoluteMagnitude = ParseDouble(raw.AbsoluteMagnitude),
                            Distance = ParseDouble(raw.DistanceLightYear),
                            Type = raw.SpectralClass ?? "",
                            Azimuth = 0,
                            Altitude = 0
                        });
                    }

                    offset += limit;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"[ERROR] Failed to fetch stars: {ex.Message}");
                    throw new Exception($"API error: {ex.Message}");
                }
            }

            return allStars;
        }

        private double ParseDouble(string value)
        {
            return double.TryParse(value, out double result) ? result : 0;
        }

        private class RawStar
        {
            public string Name { get; set; }
            public string Constellation { get; set; }
            public string RightAscension { get; set; }
            public string Declination { get; set; }
            public string ApparentMagnitude { get; set; }
            public string AbsoluteMagnitude { get; set; }
            public string DistanceLightYear { get; set; }
            public string SpectralClass { get; set; }
        }
    }
}