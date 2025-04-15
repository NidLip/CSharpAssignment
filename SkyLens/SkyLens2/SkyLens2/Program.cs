using System;
using System.IO;
using Avalonia;
using SkyLens2.Utilities;

namespace SkyLens2;

sealed class Program
{
    static Program()
    {
        try
        {
            string envFilePath = Path.Combine(AppContext.BaseDirectory, "apiKeys.env");
            EnvLoader.Load(envFilePath);
            Console.WriteLine("Environment variables loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading apiKeys.env: " + ex.Message);
        }
    }

    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}