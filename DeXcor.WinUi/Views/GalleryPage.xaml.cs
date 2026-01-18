using System;
using DeXcor.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using PexelsDotNetSDK.Models;
using Windows.System;

namespace DeXcor.WinUi.Views;

/// <summary>
/// Gallery page for browsing wallpapers.
/// </summary>
public sealed partial class GalleryPage : Microsoft.UI.Xaml.Controls.Page
{
    public GalleryViewModel ViewModel { get; } = new();

    public GalleryPage()
    {
        this.InitializeComponent();
        this.KeyDown += GalleryPage_KeyDown;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();
    }

    private void GalleryItem_Click(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Photo photo && App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateToDetail(photo);
        }
    }

    private void PhotoCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Grid grid)
        {
            var overlay = grid.FindName("QuickActionsOverlay") as Border;
            if (overlay is not null)
            {
                overlay.Visibility = Visibility.Visible;
            }
        }
    }

    private void PhotoCard_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Grid grid)
        {
            var overlay = grid.FindName("QuickActionsOverlay") as Border;
            if (overlay is not null)
            {
                overlay.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void Category_Checked(object sender, RoutedEventArgs e)
    {
        // Prevent execution during page initialization
        if (ViewModel is null) return;

        if (sender is RadioButton rb && rb.Tag is string category)
        {
            if (category == "curated")
            {
                ViewModel.SearchQuery = string.Empty;
                ViewModel.SelectedCategoryIndex = 0; // Curated is always first
            }
            else if (category == "search")
            {
                // Just switch to search mode - user will type query
                var searchIndex = Array.FindIndex(Services.PhotoCategories.All, c => c.Name == "Search");
                if (searchIndex >= 0) ViewModel.SelectedCategoryIndex = searchIndex;
            }
            else
            {
                // Set search query to category name and trigger search
                ViewModel.SearchQuery = category;
                var searchIndex = Array.FindIndex(Services.PhotoCategories.All, c => c.Name == "Search");
                if (searchIndex >= 0)
                {
                    ViewModel.SelectedCategoryIndex = searchIndex;
                }
                // Force reload with the new query
                ViewModel.LoadPhotosCommand.Execute(null);
            }
        }
    }

    #region Keyboard Shortcuts

    private async void GalleryPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        var selectedPhoto = ViewModel.SelectedPhoto;
        if (selectedPhoto is null) return;

        switch (e.Key)
        {
            case VirtualKey.W:
                // Set as wallpaper
                await App.Wallpaper.SetWallpaperAsync(selectedPhoto);
                e.Handled = true;
                break;

            case VirtualKey.D:
                // Quick download
                await App.Downloads.EnqueueAsync(selectedPhoto, "original");
                e.Handled = true;
                break;

            case VirtualKey.Enter:
                // Open detail
                if (App.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.NavigateToDetail(selectedPhoto);
                }
                e.Handled = true;
                break;

            case VirtualKey.E:
                // Edit in Ink Studio
                Frame.Navigate(typeof(InkStudioPage), selectedPhoto);
                e.Handled = true;
                break;
        }
    }

    #endregion

    #region Quick Actions

    private async void QuickSetWallpaper_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Photo photo)
        {
            await App.Wallpaper.SetWallpaperAsync(photo);
        }
    }

    private async void QuickDownload_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Photo photo)
        {
            await App.Downloads.EnqueueAsync(photo, "original");
        }
    }

    #endregion

    #region Context Menu Actions

    private async void ContextSetWallpaper_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Photo photo)
        {
            await App.Wallpaper.SetWallpaperAsync(photo);
        }
    }

    private async void ContextDownload_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Photo photo)
        {
            var size = item.Tag?.ToString() ?? "original";
            await App.Downloads.EnqueueAsync(photo, size);
        }
    }

    private void ContextEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Photo photo)
        {
            Frame.Navigate(typeof(InkStudioPage), photo);
        }
    }

    private async void ContextOpenBrowser_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Photo photo)
        {
            await Windows.System.Launcher.LaunchUriAsync(new System.Uri(photo.url));
        }
    }

    #endregion
    private void Pill_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        // VisualState handled in XAML, or add logic here
    }

    private void Pill_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        // VisualState handled in XAML, or add logic here
    }
}


