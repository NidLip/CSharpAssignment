using System.Threading.Tasks;

namespace SkyLens2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _greeting;
        public CelestialDataViewModel CelestialDataVM { get; }

        public string Greeting
        {
            get => _greeting;
            set => SetProperty(ref _greeting, value);
        }

        public MainWindowViewModel()
        {
            Greeting = "Welcome to SkyLens2!";
            CelestialDataVM = new CelestialDataViewModel();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await CelestialDataVM.LoadPlanetsAsync();
        }
    }
}