using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using SkyLens2.Models;
using SkyLens2.Services;

namespace SkyLens2.ViewModels
{
    public partial class PlanetViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Planet> planets = new();

        private readonly HorizonApiService _apiService;

        public PlanetViewModel()
        {
            _apiService = new HorizonApiService();
            LoadPlanetDataAsync();
        }

        private async Task LoadPlanetDataAsync()
        {
            var celestialBodies = await _apiService.GetPlanetsAsync();

            foreach (var body in celestialBodies)
            {
                Console.WriteLine($"[{body.Name}] Raw Description: {body.Description}");

                var radius = ParseRadius(body.Description);
                var mass = ParseMass(body.Description);
                var distance = body.EphemerisData.Distance;

                Console.WriteLine($"Parsed Radius: {radius}, Mass: {mass}, Distance: {distance}");
                
                Planets.Add(new Planet(
                    body.Name,
                    radius,
                    mass,
                    distance,
                    true
                ));
            }
        }

        private double ParseRadius(string description)
        {
            var match = Regex.Match(description, @"Radius[:=]?\s*([0-9.]+)\s*km", RegexOptions.IgnoreCase);
            return match.Success ? double.Parse(match.Groups[1].Value) : 0;
        }

        private double ParseMass(string description)
        {
            var match = Regex.Match(description, @"Mass[:=]?\s*([0-9.E+]+)\s*kg", RegexOptions.IgnoreCase);
            return match.Success ? double.Parse(match.Groups[1].Value) : 0;
        }
    }
}