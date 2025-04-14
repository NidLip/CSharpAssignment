using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SkyLens2.Models;

namespace SkyLens2.ViewModels;

public partial class PlanetViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Planet> planets;

    public PlanetViewModel()
    {
        Planets = new ObservableCollection<Planet>
        {
            new Planet("Mercury", 100, 100, 100),
            new Planet("Venus", 100, 100, 100),
            new Planet("Mars", 100, 100, 100),
            new Planet("Jupiter", 100, 100, 100),
            new Planet("Saturn", 100, 100, 100, false),
            new Planet("Uranus", 100, 100, 100, false),
            new Planet("Neptune", 100, 100, 100, false),
        };
    }
}