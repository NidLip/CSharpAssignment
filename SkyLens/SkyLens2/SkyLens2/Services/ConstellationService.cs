using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SkyLens2.Models;

namespace SkyLens2
{
    public class ConstellationService
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl =
            "https://www.datastro.eu/api/explore/v2.1/catalog/datasets/88-constellations/records?limit=88";

        public ConstellationService()
        {
            _httpClient = new HttpClient();
            FetchConstellationsAsync(); // Optional on start
        }

        public async Task FetchAndSaveRawDataAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                string rawJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine(rawJson);

                string fileName = $"constellations_raw_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(AppContext.BaseDirectory, fileName);
                await File.WriteAllTextAsync(filePath, rawJson);

                Console.WriteLine($" Raw data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error fetching constellation data: {ex.Message}");
            }
        }

        public async Task<List<Constellation>> FetchConstellationsAsync()
        {
            var constellations = new List<Constellation>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);
                var results = doc.RootElement.GetProperty("results");

                foreach (var fields in results.EnumerateArray())
                {
                    constellations.Add(new Constellation
                    {
                        IAUCode = fields.GetProperty("iau_code").GetString(),
                        LatinName = fields.GetProperty("latin_name_nom_latin").GetString(),
                        EnglishName = fields.TryGetProperty("english_name_nom_en_anglais", out var en) ? en.GetString() : null,
                        FrenchName = fields.TryGetProperty("french_name_nom_francais", out var fr) ? fr.GetString() : null,
                        Season = fields.TryGetProperty("season_saison", out var season) ? season.GetString() : null,
                        NameOrigin = fields.TryGetProperty("name_origin_origine_de_l_apellation", out var origin) ? origin.GetString() : null,
                        Declination = fields.TryGetProperty("dec_declinaison", out var dec) ? dec.GetString() : null,
                        RightAscension = fields.TryGetProperty("test", out var ra) ? ra.GetString() : null,
                        PrincipalStar = fields.TryGetProperty("principal_star_etoile_principale", out var star) ? star.GetString() : null,
                        AreaInDegrees = fields.TryGetProperty("constellation_area_in_degrees_etendue_de_la_constellation_en_degres_2", out var area)
                                        && area.TryGetDouble(out double areaDeg) ? areaDeg : 0.0,
                        AreaPercentOfCelestialSphere = fields.TryGetProperty("constellation_area_in_of_the_celestial_sphere_etendue_de_la_constellation_en_de_la_sphere_celeste", out var pct)
                                        ? pct.GetString() : null,
                        CelestialEquatorZone = fields.TryGetProperty("constellation_zone_celestial_equator_zone_de_la_constellation_equateur_celeste", out var ceq)
                                        ? ceq.GetString() : null,
                        EclipticZone = fields.TryGetProperty("constellation_zone_ecliptic_zone_de_la_constellation_ecliptique", out var ecl)
                                        ? ecl.GetString() : null,
                        MilkyWayZone = fields.TryGetProperty("constellation_zone_milky_way_zone_de_la_constellation_voie_lactee", out var mw)
                                        ? mw.GetString() : null,
                        HemisphereQuadrant = fields.TryGetProperty("quad_repere_de_l_hemisphere_et_du_quadrant", out var hemi)
                                        ? hemi.GetString() : null,
                        ImageUrl = fields.TryGetProperty("image", out var img) && img.TryGetProperty("url", out var url)
                                        ? url.GetString() : null
                    });
                }
                Console.WriteLine(results[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error parsing constellation data: {ex.Message}");
            }

            return constellations;
        }
    }
}