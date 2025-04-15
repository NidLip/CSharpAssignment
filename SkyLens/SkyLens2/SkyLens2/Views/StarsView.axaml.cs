using Avalonia.Controls;
using SkyLens2.ViewModels;

namespace SkyLens2.Views;

public partial class StarsView : UserControl
{
    public StarsView()
    {
        InitializeComponent();
        DataContext = new StarViewModel();
    }
}