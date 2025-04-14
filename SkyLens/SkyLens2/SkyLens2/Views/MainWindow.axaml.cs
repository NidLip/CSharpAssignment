    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Threading;
    using System;
    using System.Timers;
    using SkyLens2.ViewModels;

    namespace SkyLens2.Views
    {
        public partial class MainWindow : Window
        {
            public static MainWindow Instance { get; private set; }
            private Timer _timer;
            
            private ConstellationService constellationService = new ConstellationService();

            public MainWindow()
            {
                Instance = this;
                
                InitializeComponent();
                StartDateTimeUpdater();
                
                MainContent.Content = new HomeView();
                
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

            private void OnExploreSkyClick(object? sender, RoutedEventArgs e)
            {
                // TODO: Implement logic
            }

            private void OnSeeStarChartClick(object? sender, RoutedEventArgs e)
            {
                // TODO: Implement logic
            }

            private void OnNavigateConstellations(object? sender, RoutedEventArgs e)
            {
                // Only works if navigation and ConstellationsView is setup
                MainContent.Content = new ConstellationsView();
            }

            private void OnNavigatePlanets(object? sender, RoutedEventArgs e)
            {
                MainContent.Content = new PlanetsView();
            }

            private void OnNavigateStars(object? sender, RoutedEventArgs e)
            {
                MainContent.Content = new StarsView();
            }
        }
    }