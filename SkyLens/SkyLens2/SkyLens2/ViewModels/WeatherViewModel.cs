using System;
using System.Threading.Tasks;
using ReactiveUI;

namespace SkyLens2.ViewModels
{
    public partial class WeatherViewModel : ReactiveObject
    {
        private string _currentDate = "Loading...";
        private string _nextDate1 = "Loading...";
        private string _nextDate2 = "Loading...";
        private int _cloudCoverage = 0;
        private string _cloudCoverageText = "Loading...";
        private double _windSpeed = 0.0;
        private double _sunAngle;
        private string _moonPhase = "Loading...";
        private string _moonImpact = "Loading...";
        private string _pollutionLocation = "Loading...";
        private string _pollutionLevel = "Loading...";
        private string _pollutionImpact = "Loading...";
        private string _sunStage = "Loading...";
        
        public string SunStage
        {
            get => _sunStage;
            set => this.RaiseAndSetIfChanged(ref _sunStage, value);
        }

        public double SunAngle
        {
            get => _sunAngle;
            set => this.RaiseAndSetIfChanged(ref _sunAngle, value);
        }

        public string MoonPhase
        {
            get => _moonPhase;
            set => this.RaiseAndSetIfChanged(ref _moonPhase, value);
        }

        public string MoonImpact
        {
            get => _moonImpact;
            set => this.RaiseAndSetIfChanged(ref _moonImpact, value);
        }

        public string PollutionLocation
        {
            get => _pollutionLocation;
            set => this.RaiseAndSetIfChanged(ref _pollutionLocation, value);
        }

        public string PollutionLevel
        {
            get => _pollutionLevel;
            set => this.RaiseAndSetIfChanged(ref _pollutionLevel, value);
        }

        public string PollutionImpact
        {
            get => _pollutionImpact;
            set => this.RaiseAndSetIfChanged(ref _pollutionImpact, value);
        }

        public string CurrentDate
        {
            get => _currentDate;
            set => this.RaiseAndSetIfChanged(ref _currentDate, value);
        }

        public string NextDate1
        {
            get => _nextDate1;
            set => this.RaiseAndSetIfChanged(ref _nextDate1, value);
        }

        public string NextDate2
        {
            get => _nextDate2;
            set => this.RaiseAndSetIfChanged(ref _nextDate2, value);
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

        public double WindSpeed
        {
            get => _windSpeed;
            set => this.RaiseAndSetIfChanged(ref _windSpeed, value);
        }

        public WeatherViewModel()
        {
            // Start loading the weather data right after the ViewModel is created
            _ = LoadWeatherDataAsync();
        }

        public async Task LoadWeatherDataAsync()
        {
            await Task.Delay(1000); // Simulate API call

            // Get current date and next two days dynamically
            var today = DateTime.Now;
            CurrentDate = today.ToString("MMMM dd");
            NextDate1 = today.AddDays(1).ToString("MMMM dd");
            NextDate2 = today.AddDays(2).ToString("MMMM dd");

            // Example data fetching (replace with actual API calls)
            CloudCoverage = 75;  // Example cloud coverage value
            CloudCoverageText = $"Cloud Coverage: High ({CloudCoverage}%)";
            WindSpeed = 15.5;  // Example wind speed value
            
            // Example moon phase and impact
            SunAngle = 42.5;
            MoonPhase = "Waning Gibbous";
            MoonImpact = "Moderate";
            PollutionLocation = "Emmen, Netherlands";
            PollutionLevel = "Medium";
            PollutionImpact = "Moderate";
            SunStage = "Civil Twilight";
        }
    }
}
