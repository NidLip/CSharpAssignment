using System;
using System.Threading.Tasks;
using ReactiveUI;
using SkyLens2.Services;

namespace SkyLens2.ViewModels
{
    public partial class WeatherViewModel : ReactiveObject
    {
        private readonly DataAggregator _dataAggregator = new DataAggregator();
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
            try
            {
                var data = await _dataAggregator.GetAggregatedDataAsync();

                CurrentDate = DateTime.Now.ToString("MMMM dd");
                NextDate1 = DateTime.Now.AddDays(1).ToString("MMMM dd");
                NextDate2 = DateTime.Now.AddDays(2).ToString("MMMM dd");

                CloudCoverage = (int)data.CloudCoverage;
                CloudCoverageText = $"Cloud Coverage: {PollutionLevelDescription(data.CloudCoverage)} ({data.CloudCoverage}%)";
                WindSpeed = data.WindSpeed;

                SunAngle = data.SunAltitude;
                SunStage = data.StageOfNight;

                MoonPhase = data.MoonPhase;
                MoonImpact = GetMoonImpact(data.MoonPhase);

                PollutionLocation = data.LocationName;
                PollutionLevel = GetPollutionLevelDescription(data.CloudCoverage);
                PollutionImpact = GetPollutionImpact(data.CloudCoverage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading weather data:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private string GetMoonImpact(string phase)
        {
            if (phase.Contains("Full", StringComparison.OrdinalIgnoreCase)) return "High";
            if (phase.Contains("Gibbous", StringComparison.OrdinalIgnoreCase)) return "Moderate";
            if (phase.Contains("New", StringComparison.OrdinalIgnoreCase)) return "Low";
            return "Moderate";
        }

        private string PollutionLevelDescription(float coverage)
        {
            return coverage > 60 ? "High" :
                coverage > 30 ? "Medium" : "Low";
        }

        private string GetPollutionLevelDescription(float coverage)
        {
            return PollutionLevelDescription(coverage);
        }

        private string GetPollutionImpact(float coverage)
        {
            if (coverage > 60) return "Reduced Visibility";
            if (coverage > 30) return "Some Impact";
            return "Clear";
        }
    }
}
