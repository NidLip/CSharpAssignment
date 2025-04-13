using System.Collections.ObjectModel;
using SkyLens2.Models;

namespace SkyLens2.ViewModels;

public class ConstellationsViewModel : ViewModelBase
{
    public ObservableCollection<Constellation> Constellations { get; }

    public ConstellationsViewModel()
    {
        Constellations = new ObservableCollection<Constellation>
        {
            new Constellation
            {
                Name = "Orion",
                Azimuth = "120°",
                Altitude = "45°",
                NotableStars = "Betelgeuse, Rigel"
            },
            new Constellation
            {
                Name = "Ursa Major",
                Azimuth = "350°",
                Altitude = "60°",
                NotableStars = "Dubhe, Merak"
            }
            // Add more as needed or load from API/backend
        };
    }
}
