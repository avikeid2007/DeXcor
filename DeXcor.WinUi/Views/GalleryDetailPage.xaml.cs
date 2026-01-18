using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PexelsDotNetSDK.Models;

namespace DeXcor.WinUi.Views;

/// <summary>
/// Detail page for viewing a single photo.
/// </summary>
public sealed partial class GalleryDetailPage : Microsoft.UI.Xaml.Controls.Page
{
    public Photo? Photo { get; private set; }

    public GalleryDetailPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Photo photo)
        {
            Photo = photo;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (Photo is null) return;

        PhotographerText.Text = $"By {Photo.photographer}";
        DimensionsText.Text = $"{Photo.width} × {Photo.height} px";
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        if (Photo is null) return;

        var size = (sender as MenuFlyoutItem)?.Tag?.ToString() ?? "original";
        await App.Downloads.EnqueueAsync(Photo, size);

        // Show notification
        // TODO: Add toast notification
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (Photo is not null)
        {
            Frame.Navigate(typeof(InkStudioPage), Photo);
        }
    }

    private async void SetWallpaper_Click(object sender, RoutedEventArgs e)
    {
        if (Photo is null) return;

        LoadingOverlay.Visibility = Visibility.Visible;
        LoadingText.Text = "Setting wallpaper...";

        var success = await App.Wallpaper.SetWallpaperAsync(Photo);

        LoadingText.Text = success ? "Wallpaper set!" : "Failed to set wallpaper";

        await Task.Delay(1000);
        LoadingOverlay.Visibility = Visibility.Collapsed;
    }

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Navigate to previous photo in collection
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Navigate to next photo in collection
    }
}
