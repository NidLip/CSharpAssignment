using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkyLens2.Models;
using SkyLens2.Services;
using SkyLens2.Commands;

namespace SkyLens2.ViewModels
{
    public class CelestialDataViewModel : ViewModelBase
    {
        private readonly HorizonApiService _apiService;
        private bool _isLoading;
        private string _statusMessage;
        private CelestialBody _selectedBody;
        private string _searchQuery;

        public ObservableCollection<CelestialBody> CelestialBodies { get; } = new ObservableCollection<CelestialBody>();

        public ICommand SearchCommand { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public CelestialBody SelectedBody
        {
            get => _selectedBody;
            set => SetProperty(ref _selectedBody, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public CelestialDataViewModel()
        {
            _apiService = new HorizonApiService();
            SearchCommand = new RelayCommand(async _ => await SearchCelestialBodyAsync());
        }

        public async Task LoadPlanetsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading planets data...";
                
                CelestialBodies.Clear();
                var planets = await _apiService.GetPlanetsAsync();
                
                foreach (var planet in planets)
                {
                    CelestialBodies.Add(planet);
                }
                
                StatusMessage = $"Loaded {CelestialBodies.Count} celestial bodies";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchCelestialBodyAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                StatusMessage = "Please enter a search query";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = $"Searching for '{SearchQuery}'...";
                
                var body = await _apiService.GetCelestialBodyInfoAsync(SearchQuery);
                
                if (body != null && !string.IsNullOrEmpty(body.Name))
                {
                    // Check if body already exists in the collection
                    bool exists = false;
                    foreach (var existingBody in CelestialBodies)
                    {
                        if (existingBody.Id == body.Id)
                        {
                            exists = true;
                            SelectedBody = existingBody;
                            break;
                        }
                    }
                    
                    if (!exists)
                    {
                        CelestialBodies.Add(body);
                        SelectedBody = body;
                    }
                    
                    StatusMessage = $"Found: {body.Name}";
                }
                else
                {
                    StatusMessage = "No results found";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}