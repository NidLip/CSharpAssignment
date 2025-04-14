using System;
using System.Threading.Tasks;
using SkyLens2.Services;
using SkyLens2.Services.Models;
using ReactiveUI;

namespace SkyLens2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DataAggregator _aggregator;
        
        private AggregatedData _currentData;
        public AggregatedData CurrentData
        {
            get => _currentData;
            private set => this.RaiseAndSetIfChanged(ref _currentData, value);
        }

        public MainWindowViewModel()
        {
            _aggregator = new DataAggregator();
            // Start loading data as soon as the ViewModel is instantiated.
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // Use test coordinates (e.g., San Francisco) for now.
                double lat = 37.7749;
                double lon = -122.4194;
                Console.WriteLine("Starting to load aggregator data...");

                // Call the aggregator to retrieve data.
                CurrentData = await _aggregator.GetAggregatedDataAsync(lat, lon);

                // Log the results in the console for debugging purposes.
                Console.WriteLine("=== Aggregated Data Retrieved ===");
                Console.WriteLine($"Temperature: {CurrentData.Temperature} °C");
                Console.WriteLine($"Cloud Coverage: {CurrentData.CloudCoverage}%");
                Console.WriteLine($"Wind Speed: {CurrentData.WindSpeed} m/s");
                Console.WriteLine($"Sunrise: {CurrentData.Sunrise:HH:mm}");
                Console.WriteLine($"Sunset: {CurrentData.Sunset:HH:mm}");
                Console.WriteLine($"Sun Altitude: {CurrentData.SunAltitude}");
                Console.WriteLine($"Sun Azimuth: {CurrentData.SunAzimuth}");
                Console.WriteLine($"Moon Phase: {CurrentData.MoonPhase}");
                Console.WriteLine($"Stage of Night: {CurrentData.StageOfNight}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
            }
        }
    }
}