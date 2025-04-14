using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SkyLens2.Models;

namespace SkyLens2.Services
{
    public class HorizonApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ssd.jpl.nasa.gov/api/horizons.api";

        // Dictionary of known celestial bodies to avoid ambiguous queries
        private static readonly Dictionary<string, string> KnownBodies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "mercury", "199" },
            { "venus", "299" },
            { "earth", "399" },
            { "mars", "499" },
            { "jupiter", "599" },
            { "saturn", "699" },
            { "uranus", "799" },
            { "neptune", "899" },
            { "pluto", "999" },
            { "sun", "10" },
            { "moon", "301" }
        };

        public HorizonApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<string> GetCelestialBodyDataAsync(string query, DateTime startTime, DateTime endTime, string stepSize = "1d")
        {
            // Convert common planet names to their IDs
            string command = query;
            if (KnownBodies.TryGetValue(query, out string bodyId))
            {
                command = bodyId;
            }

            // Format dates in a way that the API accepts
            var startTimeStr = startTime.ToString("yyyy-MM-dd");
            var endTimeStr = endTime.ToString("yyyy-MM-dd");
            
            // URL-encode parameters to ensure special characters are transmitted correctly
            string encodedCommand = Uri.EscapeDataString(command);
            string encodedCenter = Uri.EscapeDataString("500@10");
            string encodedStartTime = Uri.EscapeDataString(startTimeStr);
            string encodedStopTime = Uri.EscapeDataString(endTimeStr);
            string encodedStepSize = Uri.EscapeDataString(stepSize);

            var relativeUrl = $"?format=json&COMMAND={encodedCommand}" +
                              $"&OBJ_DATA=YES&MAKE_EPHEM=YES" +
                              $"&EPHEM_TYPE=OBSERVER&CENTER={encodedCenter}" +
                              $"&START_TIME={encodedStartTime}&STOP_TIME={encodedStopTime}" +
                              $"&STEP_SIZE={encodedStepSize}";

            Console.WriteLine("Request URL: " + _httpClient.BaseAddress + relativeUrl);
            
            try
            {
                // Use the relative URL since HttpClient.BaseAddress is set
                var response = await _httpClient.GetStringAsync(relativeUrl);
                Console.WriteLine("Raw API Response: " + response.Substring(0, Math.Min(response.Length, 200)) + "...");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from Horizon API: {ex.Message}");
                throw;
            }
        }
        
        public async Task<List<CelestialBody>> GetPlanetsAsync()
        {
            var planets = new List<CelestialBody>();
            
            try
            {
                planets.Add(await GetCelestialBodyInfoAsync("199")); // Mercury
                planets.Add(await GetCelestialBodyInfoAsync("299")); // Venus
                planets.Add(await GetCelestialBodyInfoAsync("399")); // Earth
                planets.Add(await GetCelestialBodyInfoAsync("499")); // Mars
                planets.Add(await GetCelestialBodyInfoAsync("599")); // Jupiter
                planets.Add(await GetCelestialBodyInfoAsync("699")); // Saturn
                planets.Add(await GetCelestialBodyInfoAsync("799")); // Uranus
                planets.Add(await GetCelestialBodyInfoAsync("899")); // Neptune
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting planets: {ex.Message}");
            }
            
            return planets;
        }
        public async Task<CelestialBody> GetCelestialBodyInfoAsync(string query)
        {
    
    string bodyId = query;
    if (KnownBodies.TryGetValue(query, out string id))
    {
        bodyId = id;
    }

    var now = DateTime.Now;
    string rawResponse = "";
    
    try
    {
        rawResponse = await GetCelestialBodyDataAsync(bodyId, now, now.AddDays(1));
        
        var jsonResponse = JsonDocument.Parse(rawResponse);
        
        string result = jsonResponse.RootElement.GetProperty("result").GetString();
        Console.WriteLine("API Result Length: " + result.Length);
        
        string name = "Unknown";
        var targetNameMatch = Regex.Match(result, @"Target body name:\s+([A-Za-z\s\-]+)");
        if (targetNameMatch.Success)
        {
            name = targetNameMatch.Groups[1].Value.Trim();
        }
        
        // Create a properly initialized body object
        var body = new CelestialBody
        {
            Id = bodyId,
            Name = name,
            Description = ExtractDescription(result)
        };
        
        // Extract ephemeris data (if available)
        var ephemerisData = ParseEphemerisData(result);
        body.EphemerisData = ephemerisData;
        
        // Extract orbital period and temperature
        var orbitalPeriodMatch = Regex.Match(result, @"Orbital period\s*=\s*([0-9.]+)");
        if (orbitalPeriodMatch.Success)
        {
            body.OrbitalPeriod = orbitalPeriodMatch.Groups[1].Value + " days";
        }
        
        var tempMatch = Regex.Match(result, @"Mean Temperature\s*=\s*([0-9.]+)");
        if (tempMatch.Success)
        {
            body.SurfaceTemperature = tempMatch.Groups[1].Value + " K";
        }
        
        return body;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing '{query}': {ex.Message}");
        if (!string.IsNullOrEmpty(rawResponse))
        {
            Console.WriteLine($"Raw response snippet: {rawResponse.Substring(0, Math.Min(rawResponse.Length, 500))}");
        }
        
        return new CelestialBody
        {
            Id = bodyId,
            Name = $"Error processing '{query}'",
            Description = $"Error: {ex.Message}"
        };
    }
}

        
private EphemerisData ParseEphemerisData(string result)
{
    var ephemerisData = new EphemerisData();
    
    // Look for ephemeris table in the result
    var ephemerisMatch = Regex.Match(result, @"(?s)\$\$SOE(.*?)\$\$EOE");
    if (ephemerisMatch.Success)
    {
        string ephemerisTable = ephemerisMatch.Groups[1].Value.Trim();
        // Print the table BEFORE attempting to parse it
        Console.WriteLine("Ephemeris Table Found. First 100 characters: " + 
                         ephemerisTable.Substring(0, Math.Min(100, ephemerisTable.Length)));
        
        var lines = ephemerisTable.Split('\n');
        
        if (lines.Length > 0)
        {
            // Get the first line of data
            var line = lines[0].Trim();
            Console.WriteLine("Parsing line: " + line);
            
            // The format is a space-separated table with values in specific positions
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"Found {parts.Length} data parts");
            
            // Based on the actual format of the Horizons API response,
            // we need to extract values from specific positions
            
            // Right Ascension and Declination are typically in the first several columns
            // Format: 2025-Apr-13 00:00     00 02 33.43 -01 06 20.1  ...
            if (parts.Length >= 7)
            {
                // RA is usually in HH MM SS.SS format (parts 3, 4, 5)
                try
                {
                    // Convert HH MM SS.SS to decimal degrees
                    double raHours = double.Parse(parts[3]);
                    double raMinutes = double.Parse(parts[4]);
                    double raSeconds = double.Parse(parts[5]);
                    double rightAscension = 15 * (raHours + raMinutes/60 + raSeconds/3600); // 15 degrees per hour
                    ephemerisData.RightAscension = rightAscension;
                    Console.WriteLine($"Parsed RA: {rightAscension}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing RA: {ex.Message}");
                }
                
                // Dec is usually in DD MM SS.S format (parts 6, 7, 8)
                try
                {
                    if (parts.Length >= 9)
                    {
                        // Check sign
                        string decSign = parts[6].StartsWith("-") ? "-" : "";
                        double decDegrees = Math.Abs(double.Parse(parts[6]));
                        double decMinutes = double.Parse(parts[7]);
                        double decSeconds = double.Parse(parts[8]);
                        
                        // Convert to decimal degrees
                        double declination = (decSign == "-" ? -1 : 1) * 
                            (decDegrees + decMinutes/60 + decSeconds/3600);
                        ephemerisData.Declination = declination;
                        Console.WriteLine($"Parsed Dec: {declination}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Dec: {ex.Message}");
                }
            }
            
            // For more specific fields like distance and magnitude, we need to know their positions
            // Based on your data, these might be around positions 12-20
            
            // Try to find distance (delta) - typically around position 14-15
            for (int i = 10; i < Math.Min(parts.Length, 25); i++)
            {
                if (double.TryParse(parts[i], out double value) && value > 0 && value < 50)
                {
                    ephemerisData.Distance = value;
                    break;
                }
            }
            
            // Try to find magnitude - typically in the 25-40 range of values
            for (int i = 25; i < Math.Min(parts.Length, 40); i++)
            {
                if (double.TryParse(parts[i], out double value) && value > 0 && value < 15)
                {
                    // Most planets have magnitude between 0 and 15
                    ephemerisData.Magnitude = value;
                    Console.WriteLine($"Found magnitude at position {i}: {value}");
                    break;
                }
            }
            
            // Alternative: look for specific columns by searching for headers
            bool foundValues = false;
            foreach (var headerLine in result.Split('\n'))
            {
                if (headerLine.Contains("R.A.") && headerLine.Contains("DEC") && 
                    headerLine.Contains("delta") && headerLine.Contains("mag"))
                {
                    Console.WriteLine("Found header line: " + headerLine);
                    foundValues = true;
                    break;
                }
            }
            
            if (!foundValues)
            {
                Console.WriteLine("Could not find column headers to parse data");
            }
        }
    }
    else
    {
        Console.WriteLine("No ephemeris table found in API response");
    }
    
    // Log final parsed values
    Console.WriteLine($"Final data: RA={ephemerisData.RightAscension}, " +
                     $"Dec={ephemerisData.Declination}, " +
                     $"Dist={ephemerisData.Distance}, " +
                     $"Mag={ephemerisData.Magnitude}");
    
    return ephemerisData;
}

        
        private string ExtractDescription(string result)
{
    var descriptions = new List<string>();
    
    try
    {
        // Extract radius with multiple pattern attempts
        var radiusPatterns = new[] {
            @"Mean radius \(km\)\s*=\s*([0-9.]+)",
            @"Vol\. Mean Radius \(km\)\s*=\s*([0-9.]+)",
            @"Equatorial radius \(km\)\s*=\s*([0-9.]+)",
            @"Volumetric mean radius \(km\)\s*=\s*([0-9.]+)"
        };
        
        bool foundRadius = false;
        foreach (var pattern in radiusPatterns)
        {
            var match = Regex.Match(result, pattern);
            if (match.Success)
            {
                descriptions.Add($"Radius:{match.Groups[1].Value} km");
                foundRadius = true;
                break;
            }
        }
        
        if (!foundRadius)
        {
            descriptions.Add("Radius:Not available in API response");
        }
        
        // Extract mass with improved scientific notation handling
        bool foundMass = false;
        
        // Format 1: "Mass x10^n (kg) = x.xx"
        var massPattern1 = @"Mass\s*x10\^(\d+)\s*\(kg\)\s*=\s*([0-9.]+)";
        var massMatch1 = Regex.Match(result, massPattern1);
        if (massMatch1.Success && massMatch1.Groups.Count >= 3)
        {
            string exponent = massMatch1.Groups[1].Value;
            string baseValue = massMatch1.Groups[2].Value;
            descriptions.Add($"Mass:{baseValue}E+{exponent} kg");
            foundMass = true;
        }
        
        // Format 2: "Mass (10^n kg) = x.xx"
        if (!foundMass)
        {
            var massPattern2 = @"Mass\s*\(10\^(\d+)\s*kg\)\s*=\s*([0-9.]+)";
            var massMatch2 = Regex.Match(result, massPattern2);
            if (massMatch2.Success && massMatch2.Groups.Count >= 3)
            {
                string exponent = massMatch2.Groups[1].Value;
                string baseValue = massMatch2.Groups[2].Value;
                descriptions.Add($"Mass:{baseValue}E+{exponent} kg");
                foundMass = true;
            }
        }
        
        // Format 3: "Mass GM (km^3/s^2) = x.xx x 10^n"
        if (!foundMass)
        {
            var massPattern3 = @"Mass GM \(km\^3/s\^2\)\s*=\s*([0-9.]+)\s*x\s*10\^(\d+)";
            var massMatch3 = Regex.Match(result, massPattern3);
            if (massMatch3.Success && massMatch3.Groups.Count >= 3)
            {
                string baseValue = massMatch3.Groups[1].Value;
                string exponent = massMatch3.Groups[2].Value;
                descriptions.Add($"Mass:{baseValue}E+{exponent} kg");
                foundMass = true;
            }
        }
        
        // Try any generic format with scientific notation
        if (!foundMass)
        {
            var massGenericPattern = @"Mass.*?([0-9.]+)\s*[xX]\s*10\^(\d+)";
            var massGenericMatch = Regex.Match(result, massGenericPattern);
            if (massGenericMatch.Success && massGenericMatch.Groups.Count >= 3)
            {
                string baseValue = massGenericMatch.Groups[1].Value;
                string exponent = massGenericMatch.Groups[2].Value;
                descriptions.Add($"Mass:{baseValue}E+{exponent} kg");
                foundMass = true;
            }
        }
        
        if (!foundMass)
        {
            descriptions.Add("Mass:Not available in API response");
        }
        
        // Extract distance from sun (semi-major axis)
        var distancePatterns = new[] {
            @"Semi-major axis\s*\(AU\)\s*=\s*([0-9.]+)",
            @"semi-major axis\s*=\s*([0-9.]+)",
            @"a\s*\(AU\)\s*=\s*([0-9.]+)",
            @"mean distance.*?=\s*([0-9.]+)\s*AU"
        };
        
        bool foundDistance = false;
        foreach (var pattern in distancePatterns)
        {
            var match = Regex.Match(result, pattern);
            if (match.Success)
            {
                descriptions.Add($"Distance from Sun:{match.Groups[1].Value} AU");
                foundDistance = true;
                break;
            }
        }
        
        if (!foundDistance)
        {
            descriptions.Add("Distance from Sun:Not available in API response");
        }
        
        // Add debug output to see what's in the API response
        Console.WriteLine("API Response Excerpt (for Mass detection):");
        // Find position of "Mass" in the result
        int massPos = result.IndexOf("Mass");
        if (massPos >= 0)
        {
            // Extract a chunk of text around "Mass" for debugging
            int start = Math.Max(0, massPos - 10);
            int length = Math.Min(100, result.Length - start);
            Console.WriteLine(result.Substring(start, length));
        }
    }
    catch (Exception ex)
    {
        return $"Error parsing data: {ex.Message}";
    }
    
    return string.Join("\n", descriptions);
}

        
        public async Task<List<CelestialBody>> SearchAsync(string query)
        {
            // First try to get direct body info
            var body = await GetCelestialBodyInfoAsync(query);
            
            if (body.Name != "Unknown" && !body.Name.Contains("Error") && !body.Name.Contains("Multiple matches"))
            {
                // If we got a valid result, return it
                return new List<CelestialBody> { body };
            }
            else if (body.Name.Contains("Multiple matches"))
            {
                // Parse multiple matches from the response
                var now = DateTime.Now;
                var rawResponse = await GetCelestialBodyDataAsync(query, now, now.AddDays(1));
                var jsonResponse = JsonDocument.Parse(rawResponse);
                string result = jsonResponse.RootElement.GetProperty("result").GetString();
                
                var matches = new List<CelestialBody>();
                var matchPattern = @"(\s+)(\d+|-\d+)(\s+)([^\n]+)";
                var matchRegex = new Regex(matchPattern);
                var matchResults = matchRegex.Matches(result);
                
                foreach (Match match in matchResults)
                {
                    if (match.Groups.Count >= 5)
                    {
                        var matchedId = match.Groups[2].Value.Trim();
                        var matchedName = match.Groups[4].Value.Trim();
                        
                        matches.Add(new CelestialBody 
                        { 
                            Id = matchedId, 
                            Name = matchedName,
                            Description = $"ID: {matchedId}"
                        });
                    }
                }
                
                return matches; 
            }
            
            // If nothing found, return empty list
            return new List<CelestialBody>();
        }
    }
}