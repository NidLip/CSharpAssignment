using Avalonia.Controls;
using SkyLens2.ViewModels;

namespace SkyLens2.Views
{
    public partial class WeatherView : UserControl
    {
        public WeatherView()
        {
            InitializeComponent();
            DataContext = new WeatherViewModel(); // Bind the ViewModel
        }
    }
}