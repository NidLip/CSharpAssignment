using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Timers;
using SkyLens2.ViewModels;
using SkyLens2.Views;

namespace SkyLens2.Views
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        private Timer _timer;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();
            StartDateTimeUpdater();

            MainContent.Content = new HomeView(); // Load HomeView by default
        }

        private void StartDateTimeUpdater()
        {
            _timer = new Timer(60000); // update every minute
            _timer.Elapsed += (s, e) => UpdateDateTime();
            _timer.Start();
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var now = DateTime.Now;
                var formatted = now.ToString("yyyy/MM/dd hh:mm tt");
                DateTimeText.Text = formatted;
            });
        }

        private void OnNavigateHome(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new HomeView();
        }

        private void OnNavigateWeather(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new WeatherView();
        }

        private void OnNavigatePlanets(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new PlanetsView();
        }

        private void OnNavigateConstellations(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new ConstellationsView();
        }

        private void OnNavigateStars(object? sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new StarsView();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load StarsView: {ex.Message}");
            }
        }
    }
}