using System;
using System.Threading.Tasks;

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
            // Start loading the data right after the ViewModel is created
            _ = LoadWeatherDataAsync(); // Fire-and-forget async call
        }
        
        public async Task LoadWeatherDataAsync()
        {
            await Task.Delay(1000); // Simulate API call

            MoonPhase = "Waxing Gibbous";
            ObservationOutlook = "Favorable";
            OutlookReason = "Clear skies and moderate light pollution";
            RecommendedObservationTime = "22:15 - 03:30";
            CloudCoverage = 12;
            CloudCoverageText = "Clear (12%)";
            LightPollution = 35;
            LightPollutionText = "Medium (35%)";
            AirTurbulence = "Stable";
        }
    }
}
