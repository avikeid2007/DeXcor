using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PexelsDotNetSDK.Models;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace DeXcor.WinUi.Views;

/// <summary>
/// Ink Studio for editing wallpapers (simplified version).
/// </summary>
public sealed partial class InkStudioPage : Microsoft.UI.Xaml.Controls.Page
{
    private Photo? _photo;
    private StorageFile? _backgroundFile;

    public bool ShowInkFeature => true;

    public InkStudioPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Photo photo)
        {
            _photo = photo;
            await LoadPhotoAsync(photo);
        }
        else
        {
            EmptyState.Visibility = Visibility.Visible;
        }
    }

    private async System.Threading.Tasks.Task LoadPhotoAsync(Photo photo)
    {
        try
        {
            // Download image for editing
            _backgroundFile = await App.Wallpaper.DownloadImageAsync(
                photo.source.large2x,
                $"ink_edit_{photo.id}.jpg");

            if (_backgroundFile is not null)
            {
                using var stream = await _backgroundFile.OpenAsync(FileAccessMode.Read);
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                BackgroundImage.Source = bitmap;
                EmptyState.Visibility = Visibility.Collapsed;
            }
        }
        catch (System.Exception)
        {
            EmptyState.Visibility = Visibility.Visible;
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack) Frame.GoBack();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implement save with Win2D
        await System.Threading.Tasks.Task.CompletedTask;
    }

    private async void SetWallpaper_Click(object sender, RoutedEventArgs e)
    {
        if (_backgroundFile is not null)
        {
            await App.Wallpaper.SetWallpaperFromFileAsync(_backgroundFile);
        }
        else if (_photo is not null)
        {
            await App.Wallpaper.SetWallpaperAsync(_photo);
        }
    }

    private async void LoadImage_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");

        // Get the window handle
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file is not null)
        {
            _backgroundFile = file;
            using var stream = await file.OpenAsync(FileAccessMode.Read);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            BackgroundImage.Source = bitmap;
            EmptyState.Visibility = Visibility.Collapsed;
        }
    }
}
