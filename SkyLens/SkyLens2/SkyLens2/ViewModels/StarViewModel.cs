using System.Collections.ObjectModel;
using SkyLens2.Models;

namespace SkyLens2.ViewModels;

public class StarViewModel
{
    public ObservableCollection<Star> Stars { get; set; } = new()
    {
        new Star
        {
            Name = "Sirius",
            Type = "Blue-white Main Sequence",
            Distance = 8.6,
            AbsoluteMagnitude = 1.4,
            ApparentMagnitude = -1.46,
            Azimuth = 180,
            Altitude = 45,
            Constellation = "Canis Major"
        },
        new Star
        {
            Name = "Betelgeuse",
            Type = "Red Supergiant",
            Distance = 642.5,
            AbsoluteMagnitude = -5.85,
            ApparentMagnitude = 0.42,
            Azimuth = 210,
            Altitude = 32,
            Constellation = "Orion"
        }
    };
}
