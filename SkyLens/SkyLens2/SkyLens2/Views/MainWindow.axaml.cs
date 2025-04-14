using Avalonia.Controls;
using SkyLens2.ViewModels;

namespace SkyLens2.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}