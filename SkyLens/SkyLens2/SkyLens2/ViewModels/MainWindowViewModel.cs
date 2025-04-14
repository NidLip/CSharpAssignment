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

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            private set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public MainWindowViewModel()
        {
            _aggregator = new DataAggregator();
            StatusMessage = "Loading data...";
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                Console.WriteLine("LoadDataAsync triggered. Fetching aggregator data...");
                CurrentData = await _aggregator.GetAggregatedDataAsync();
                Console.WriteLine("Data retrieved from aggregator.");
                Console.WriteLine($"Temperature: {CurrentData.Temperature} °C");
                StatusMessage = "Data loaded successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
                StatusMessage = $"Error: {ex.Message}";
            }
        }
    }
}