using System;
using Avalonia.Controls;
using SkyLens2.ViewModels;

namespace SkyLens2.Views;

public partial class ConstellationsView : UserControl
{
    public ConstellationsView()
    {
        InitializeComponent();
        
        var vm = new ConstellationsViewModel();
        this.DataContext = vm;
        
        Console.WriteLine("Constellations view: " + vm.Constellations.Count);
    }
}