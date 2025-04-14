using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SkyLens2.Models;

namespace SkyLens2.ViewModels;

public class ConstellationsViewModel : ViewModelBase
{
    public ObservableCollection<Constellation> Constellations { get; } = new ();
    

    public ConstellationsViewModel()
    {
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var service = new ConstellationService();
        var data = await service.FetchConstellationsAsync();

        Constellations.Clear();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            Constellations.Clear();
            foreach (var constellation in data)
            {
                Constellations.Add(constellation);
            }

            Console.WriteLine("Constellations view: " + Constellations.Count);
        });
    }
}