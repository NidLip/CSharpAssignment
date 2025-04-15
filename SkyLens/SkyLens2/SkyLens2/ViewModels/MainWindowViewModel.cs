using System;
using System.Threading.Tasks;
using SkyLens2.Services;

using ReactiveUI;

namespace SkyLens2.ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject
    {
        private string _moonPhase = "Loading...";
        private string _observationOutlook = "Loading...";
        private string _outlookReason = "Loading...";
        private string _recommendedObservationTime = "Loading...";
        private int _cloudCoverage = 0;
        private string _cloudCoverageText = "Loading...";
        private int _lightPollution = 0;
        private string _lightPollutionText = "Loading...";
        private string _airTurbulence = "Loading...";
        
        private readonly DataAggregator _aggregator = new DataAggregator();

        public string MoonPhase
        {
            get => _moonPhase;
            set => this.RaiseAndSetIfChanged(ref _moonPhase, value);
        }

        public string ObservationOutlook
        {
            get => _observationOutlook;
            set => this.RaiseAndSetIfChanged(ref _observationOutlook, value);
        }

        public string OutlookReason
        {
            get => _outlookReason;
            set => this.RaiseAndSetIfChanged(ref _outlookReason, value);
        }

        public string RecommendedObservationTime
        {
            get => _recommendedObservationTime;
            set => this.RaiseAndSetIfChanged(ref _recommendedObservationTime, value);
        }

        public int CloudCoverage
        {
            get => _cloudCoverage;
            set => this.RaiseAndSetIfChanged(ref _cloudCoverage, value);
        }

        public string CloudCoverageText
        {
            get => _cloudCoverageText;
            set => this.RaiseAndSetIfChanged(ref _cloudCoverageText, value);
        }

        public int LightPollution
        {
            get => _lightPollution;
            set => this.RaiseAndSetIfChanged(ref _lightPollution, value);
        }

        public string LightPollutionText
        {
            get => _lightPollutionText;
            set => this.RaiseAndSetIfChanged(ref _lightPollutionText, value);
        }

        public string AirTurbulence
        {
            get => _airTurbulence;
            set => this.RaiseAndSetIfChanged(ref _airTurbulence, value);
        }
        public MainWindowViewModel()
        {
            _ = LoadWeatherDataAsync();
        }
        
        public async Task LoadWeatherDataAsync()
        {
            try
            {
                var data = await _aggregator.GetAggregatedDataAsync();

                MoonPhase = data.MoonPhase;

                CloudCoverage = (int)data.CloudCoverage;
                CloudCoverageText = $"{GetCloudLabel(CloudCoverage)} ({CloudCoverage}%)";

                LightPollution = CloudCoverage > 60 ? 70 : 35;
                LightPollutionText = $"{GetPollutionLabel(LightPollution)} ({LightPollution}%)";

                ObservationOutlook = (CloudCoverage < 30 && LightPollution < 40)
                    ? "Favorable"
                    : "Limited";

                OutlookReason = (CloudCoverage < 30)
                    ? "Clear skies"
                    : "Cloudy or light-polluted skies";

                RecommendedObservationTime = "22:00 - 03:00";

                AirTurbulence = data.WindSpeed switch
                {
                    < 5 => "Stable",
                    < 15 => "Moderate",
                    _ => "Unstable"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainWindowViewModel] Error loading weather data: {ex.Message}");
                ObservationOutlook = "Unavailable";
            }
        }
        
        private string GetCloudLabel(int percentage)
        {
            return percentage switch
            {
                <= 20 => "Clear",
                <= 50 => "Partly Cloudy",
                <= 80 => "Mostly Cloudy",
                _ => "Overcast"
            };
        }

        private string GetPollutionLabel(int level)
        {
            return level switch
            {
                <= 30 => "Low",
                <= 60 => "Moderate",
                _ => "High"
            };
        }
    }
}
