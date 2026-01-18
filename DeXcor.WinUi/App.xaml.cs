using DeXcor.WinUi.Services;

using Microsoft.UI.Xaml;

using System.Threading.Tasks;

namespace DeXcor.WinUi;

/// <summary>
/// DeXcor WinUI Application
/// </summary>
public partial class App : Application
{
    public static MainWindow? MainWindow { get; private set; }

    // Services (simple singleton pattern - consider DI for larger apps)
    public static NavigationService Navigation { get; } = new();
    public static ThemeSelectorService ThemeSelector { get; } = new();
    public static PexelsService Pexels { get; } = new();
    public static WallpaperService Wallpaper { get; } = new();
    public static VideoWallpaperService VideoWallpaper { get; } = new();
    public static DownloadManager Downloads { get; } = new();

    public App()
    {
        this.InitializeComponent();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Initialize theme
        await ThemeSelector.InitializeAsync();

        // Initialize Pexels API
        // TODO: Get API key from settings or prompt user
        var apiKey = await LoadApiKeyAsync();
        if (!string.IsNullOrEmpty(apiKey))
        {
            Pexels.Initialize(apiKey);
        }

        // Create and activate main window
        MainWindow = new MainWindow();
        MainWindow.Activate();

        // Apply theme
        ThemeSelector.ApplyTheme();
    }

    private static async Task<string> LoadApiKeyAsync()
    {
        // Try to load from settings
        var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
        if (settings.Values.TryGetValue("PexelsApiKey", out var key) && key is string apiKey)
        {
            return apiKey;
        }

        // Return empty - user will need to set it in settings
        return string.Empty;
    }

    public static async Task SaveApiKeyAsync(string apiKey)
    {
        var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
        settings.Values["PexelsApiKey"] = apiKey;

        if (!string.IsNullOrEmpty(apiKey))
        {
            Pexels.Initialize(apiKey);
        }

        await Task.CompletedTask;
    }
}
