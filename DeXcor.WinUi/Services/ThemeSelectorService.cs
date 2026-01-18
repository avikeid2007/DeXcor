using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace DeXcor.WinUi.Services;

/// <summary>
/// Service for managing app theme (Light/Dark/System).
/// </summary>
public class ThemeSelectorService
{
    private const string SettingsKey = "AppTheme";

    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;
        await SaveThemeToSettingsAsync(theme);
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        if (App.MainWindow?.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;
        }
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var themeName = await Task.Run(() =>
            ApplicationData.Current.LocalSettings.Values[SettingsKey] as string);

        if (Enum.TryParse<ElementTheme>(themeName, out var theme))
        {
            return theme;
        }

        return ElementTheme.Default;
    }

    private async Task SaveThemeToSettingsAsync(ElementTheme theme)
    {
        await Task.Run(() =>
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString());
    }
}
