using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using SkyLens2.Models;
using SkyLens2.Services;

namespace SkyLens2.ViewModels
{
    public class StarViewModel : ReactiveObject
    {
        private readonly StarService _starService;

        public ObservableCollection<Star> Stars { get; } = new();

        private string _statusMessage = "Loading...";
        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public ReactiveCommand<Unit, Unit> LoadStarsCommand { get; }

        public StarViewModel()
        {
            _starService = new StarService();
            LoadStarsCommand = ReactiveCommand.CreateFromTask(LoadStarsAsync);
            LoadStarsCommand.Execute().Subscribe(); // Load on init
        }

        private async Task LoadStarsAsync()
        {
            try
            {
                Stars.Clear();
                StatusMessage = "Fetching star data...";

                var result = await _starService.GetStarsAsync();

                foreach (var star in result)
                    Stars.Add(star);

                StatusMessage = $"Loaded {Stars.Count} stars.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }
    }
}