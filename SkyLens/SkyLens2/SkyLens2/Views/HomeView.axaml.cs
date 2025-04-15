using Avalonia.Controls;
using Avalonia.Interactivity;
using SkyLens2.ViewModels;

namespace SkyLens2.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            
            var viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;

            // Optionally trigger fake loading for now
            _ = viewModel.LoadWeatherDataAsync();
        }
        
        private void OnNavigateConstellations(object? sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainContent.Content = new ConstellationsView();
        }

        private void OnNavigatePlanets(object? sender, RoutedEventArgs e)
        {
           MainWindow.Instance.MainContent.Content = new PlanetsView();
        }

        private void OnNavigateStars(object? sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainContent.Content = new StarsView();
        }
    }
}