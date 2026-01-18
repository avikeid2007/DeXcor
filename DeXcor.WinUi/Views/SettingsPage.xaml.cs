using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;

namespace DeXcor.WinUi.Views;

/// <summary>
/// Settings page for app configuration.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        // Set version text
        var version = Package.Current.Id.Version;
        VersionText.Text = $"Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        // Set current theme
        var currentTheme = App.ThemeSelector.Theme;
        foreach (var item in ThemeRadioButtons.Items.Cast<RadioButton>())
        {
            if (item.Tag?.ToString() == currentTheme.ToString())
            {
                item.IsChecked = true;
                break;
            }
        }

        // Set API key if available
        if (App.Pexels.IsInitialized)
        {
            ApiKeyBox.Password = "••••••••••••••••"; // Masked
        }
    }

    private async void SaveApiKey_Click(object sender, RoutedEventArgs e)
    {
        var apiKey = ApiKeyBox.Password;
        if (!string.IsNullOrWhiteSpace(apiKey) && !apiKey.Contains("•"))
        {
            await App.SaveApiKeyAsync(apiKey);

            // Show success
            var dialog = new ContentDialog
            {
                Title = "API Key Saved",
                Content = "Your Pexels API key has been saved successfully.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    private async void ThemeRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeRadioButtons.SelectedItem is RadioButton selected && selected.Tag is string themeTag)
        {
            var theme = themeTag switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };

            await App.ThemeSelector.SetThemeAsync(theme);
        }
    }

    private async void OpenDownloadsFolder_Click(object sender, RoutedEventArgs e)
    {
        await App.Downloads.OpenDownloadsFolderAsync();
    }
}
