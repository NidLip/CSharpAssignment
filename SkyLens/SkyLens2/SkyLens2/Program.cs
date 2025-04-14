using Avalonia;
using DotNetEnv;
using System;
using System.IO;
using SkyLens2.Utilities;

namespace SkyLens2;

sealed class Program
{
    // The static constructor loads the API keys before Main is called.
    static Program()
    {
        // Load environment variables from the file "apiKeys.env"
        // The file should be at the project root (or adjust the path accordingly).
        try
        {
            string envFilePath = Path.Combine(AppContext.BaseDirectory, "apiKeys.env");
            EnvLoader.Load("apiKeys.env");
            Console.WriteLine("Environment variables loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading apiKeys.env: " + ex.Message);
        }
    }

    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}