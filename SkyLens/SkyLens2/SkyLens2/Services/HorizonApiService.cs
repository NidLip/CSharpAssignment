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
            string encodedCenter = Uri.EscapeDataString("500@399");
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
            // Convert common planet names to their IDs
            string bodyId = query;
            if (KnownBodies.TryGetValue(query, out string id))
            {
                bodyId = id;
            }

            var now = DateTime.Now;
            var rawResponse = await GetCelestialBodyDataAsync(bodyId, now, now.AddDays(1));
            
            try
            {
                // Parse JSON response
                var jsonResponse = JsonDocument.Parse(rawResponse);
                
                // Extract result string
                string result = jsonResponse.RootElement.GetProperty("result").GetString();
                
                // Check if there are multiple matches
                if (result.Contains("Multiple major-bodies match"))
                {
                    // Extract the matches
                    var matches = new List<CelestialBody>();
                    
                    // Use regex to extract ID and name pairs
                    var matchPattern = @"(\d+|-\d+)\s+([^\n]+)";
                    var matchRegex = new Regex(matchPattern);
                    var matchResults = matchRegex.Matches(result);
                    foreach (Match match in matchResults)
                    {
                        
                        if (match.Groups.Count >= 3)
                        {
                            var matchedId = match.Groups[1].Value.Trim();
                            var matchedName = match.Groups[2].Value.Trim();
                            
                            matches.Add(new CelestialBody 
                            { 
                                Id = matchedId, 
                                Name = matchedName,
                                Description = $"ID: {matchedId}"
                            });
                        }
                    }
                    
                    if (matches.Count > 0)
                    {
                        // Return the first match for now
                        // In a real app, you'd want to let the user select from the matches
                        Console.WriteLine($"Found {matches.Count} matches for '{query}'. Using the first match.");
                        return matches[0];
                    }
                    
                    
                    return new CelestialBody
                    {
                        Id = bodyId,
                        Name = $"Multiple matches for '{query}'",
                        Description = "Please use a specific ID instead."
                    };
                }
                var body = new CelestialBody();

                // Extract celestial body name from the result string
                string name = "Unknown";
                var targetNameMatch = Regex.Match(result, @"Target body name:\s+([^\n(]+)");
                if (targetNameMatch.Success)
                {
                    name = targetNameMatch.Groups[1].Value.Trim();
                }
                
                // Extract ephemeris data (if available)
                var ephemerisData = ParseEphemerisData(result);
                // Add these lines here to extract orbital period and temperature
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
                
                return new CelestialBody
                {
                    Id = bodyId,
                    Name = name,
                    Description = "",
                    EphemerisData = ephemerisData,
                    OrbitalPeriod = body?.OrbitalPeriod, 
                    SurfaceTemperature = body?.SurfaceTemperature
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing response for query '{query}': {ex.Message}");
                Console.WriteLine($"Raw response snippet: {rawResponse.Substring(0, Math.Min(rawResponse.Length, 500))}");
                    
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
                var lines = ephemerisTable.Split('\n');
                
                if (lines.Length > 0)
                {
                    // Try to determine format by counting commas
                    var line = lines[0].Trim();
                    
                    if (line.Contains(","))
                    {
                        // Comma-separated format
                        var dataParts = line.Split(',');
                        if (dataParts.Length >= 4)
                        {
                            // Try to parse RA, Dec, Distance, etc.
                            double ra = 0, dec = 0, dist = 0, mag = 0;
                            
                            // The format might vary, so we need to search for relevant data
                            foreach (var part in dataParts)
                            {
                                // Try to parse the number from the part
                                if (double.TryParse(part.Trim(), out double value))
                                {
                                    // Assign values based on position
                                    // This is a simplification; actual format might differ
                                    if (ra == 0) ra = value;
                                    else if (dec == 0) dec = value;
                                    else if (dist == 0) dist = value;
                                    else if (mag == 0) mag = value;
                                }
                            }
                            
                            ephemerisData.RightAscension = ra;
                            ephemerisData.Declination = dec;
                            ephemerisData.Distance = dist;
                            ephemerisData.Magnitude = mag;
                        }
                    }
                    else
                    {
                        // Space-separated format
                        // Extract values based on position or labels
                        // This will depend on the exact format returned by the API
                        
                        var raMatch = Regex.Match(line, @"R\.A\.\s*\(ICRF\)=\s*([0-9.]+)");
                        var decMatch = Regex.Match(line, @"DEC\s*\(ICRF\)=\s*([0-9.]+)");
                        var distMatch = Regex.Match(line, @"delta\s*=\s*([0-9.]+)");
                        var magMatch = Regex.Match(line, @"APmag\s*=\s*([0-9.]+)");
                        
                        if (raMatch.Success) 
                        {
                            double tempRA;
                            if (double.TryParse(raMatch.Groups[1].Value, out tempRA))
                                ephemerisData.RightAscension = tempRA;
                        }

                        if (decMatch.Success) 
                        {
                            double tempDec;
                            if (double.TryParse(decMatch.Groups[1].Value, out tempDec))
                                ephemerisData.Declination = tempDec;
                        }

                        if (distMatch.Success) 
                        {
                            double tempDist;
                            if (double.TryParse(distMatch.Groups[1].Value, out tempDist))
                                ephemerisData.Distance = tempDist;
                        }

                        if (magMatch.Success) 
                        {
                            double tempMag;
                            if (double.TryParse(magMatch.Groups[1].Value, out tempMag))
                                ephemerisData.Magnitude = tempMag;
                        }
                    }Console.WriteLine("Ephemeris Table: " + ephemerisTable);

                }
            }
            
            return ephemerisData;
        }
        
        private string ExtractDescription(string result)
        {
            // Extract relevant information from the result text
            var descriptions = new List<string>();
            
            var radiusMatch = Regex.Match(result, @"Mean radius \(km\).*?=\s+([0-9.]+)");
            if (radiusMatch.Success)
            {
                descriptions.Add($"Radius: {radiusMatch.Groups[1].Value} km");
            }
            
            var massMatch = Regex.Match(result, @"Mass.*?=\s+([0-9.]+)");
            if (massMatch.Success)
            {
                descriptions.Add($"Mass: {massMatch.Groups[1].Value} kg");
            }
            
            // Extract other interesting properties
            var rotPeriodMatch = Regex.Match(result, @"rot\. period\s*=\s*([0-9.]+)");
            if (rotPeriodMatch.Success)
            {
                descriptions.Add($"Rotation period: {rotPeriodMatch.Groups[1].Value} hours");
            }
            
            if (descriptions.Count == 0)
            {
                // If no specific properties found, provide a generic description
                return "No detailed information available";
            }
            
            return string.Join(", ", descriptions);
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