using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkyLens2.Services.Models;
using SkyLens2.Utilities;

namespace SkyLens2.Services
{
    public class AstronomyService
    {
        private HttpClient _httpClient;

        // Parameterless constructor that loads the required env variables.
        public AstronomyService()
        {
            // Load env variables if not already set.
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AstronomyAppId")) ||
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AstronomyAppSecret")))
            {
                EnvLoader.Load("apiKeys.env");
            }

            string appId = Environment.GetEnvironmentVariable("AstronomyAppId");
            string appSecret = Environment.GetEnvironmentVariable("AstronomyAppSecret");

            // Debug: log loaded keys.
            Console.WriteLine($"AstronomyAppId: {appId}");
            Console.WriteLine($"AstronomyAppSecret: {appSecret}");

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret))
            {
                throw new Exception("Astronomy API credentials are not set.");
            }

            _httpClient = new HttpClient();
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{appId}:{appSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Combined endpoint: retrieves both Sun and Moon data.
        public async Task<AstronomyData> GetAstronomyDataAsync(double lat, double lon)
        {
            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            // Convert coordinates using invariant culture.
            string latStr = lat.ToString(CultureInfo.InvariantCulture);
            string lonStr = lon.ToString(CultureInfo.InvariantCulture);
            // Add required "time" parameter. Here we use 12:00:00.
            string url = $"https://api.astronomyapi.com/api/v2/bodies/positions?latitude={latStr}&longitude={lonStr}&from_date={date}&to_date={date}&time=12:00:00&elevation=0.0";

            Console.WriteLine("Fetching Astronomy data with URL:");
            Console.WriteLine(url);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                // Read error details from the response.
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Astronomy API error: {response.StatusCode}, details: {errorContent}");
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Astronomy API raw response:");
            Console.WriteLine(json);

            dynamic data = JsonConvert.DeserializeObject(json)
                           ?? throw new Exception("Failed to deserialize JSON from Astronomy API.");

            // Assume index 0 contains Sun data and index 1 contains Moon data.
            var sunData = data.data.table.rows[0].cells[0];
            var moonData = data.data.table.rows[0].cells[1];

            AstronomyData astronomy = new AstronomyData
            {
                Sunrise = DateTime.Parse((string)sunData.rise.apparent),
                Sunset = DateTime.Parse((string)sunData.set.apparent),
                SunAltitude = (double)sunData.position.horizontal.altitude,
                SunAzimuth = (double)sunData.position.horizontal.azimuth,
                MoonPhase = (string)moonData.phase.name
            };

            return astronomy;
        }
    }
}