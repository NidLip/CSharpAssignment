using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SkyLens2.ViewModels;

namespace SkyLens2.Views;

public partial class PlanetsView : UserControl
{
    public PlanetsView()
    {
        InitializeComponent();
        DataContext = new PlanetViewModel();
    }
}