using System;
using System.Linq;
using DeXcor.WinUi.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DeXcor.WinUi;

/// <summary>
/// Main shell window with NavigationView.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        // Set up custom title bar
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Apply Mica backdrop
        TrySetMicaBackdrop();

        // Initialize navigation
        App.Navigation.Initialize(ContentFrame, NavView);

        // Set initial page
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void TrySetMicaBackdrop()
    {
        // Use DesktopAcrylic for a visible frosted glass effect
        if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
        {
            var acrylicBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
            this.SystemBackdrop = acrylicBackdrop;
        }
        else if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            // Fallback to Mica if Acrylic not supported
            var micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop
            {
                Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
            };
            this.SystemBackdrop = micaBackdrop;
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            NavigateToPage(tag);
        }
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
            UpdateSelectedItemAfterNavigation();
        }
    }

    private void NavigateToPage(string? tag)
    {
        Type? pageType = tag switch
        {
            "Home" => typeof(MainPage),
            "Gallery" => typeof(GalleryPage),
            "Collections" => typeof(CollectionsPage),
            "Downloads" => typeof(DownloadsPage),
            "InkStudio" => typeof(InkStudioPage),
            "Settings" => typeof(SettingsPage),
            _ => null
        };

        if (pageType is not null && ContentFrame.CurrentSourcePageType != pageType)
        {
            ContentFrame.Navigate(pageType);
        }
    }

    private void UpdateSelectedItemAfterNavigation()
    {
        var currentPage = ContentFrame.CurrentSourcePageType;
        string tag = currentPage?.Name switch
        {
            nameof(MainPage) => "Home",
            nameof(GalleryPage) => "Gallery",
            nameof(CollectionsPage) => "Collections",
            nameof(DownloadsPage) => "Downloads",
            nameof(InkStudioPage) => "InkStudio",
            nameof(SettingsPage) => "Settings",
            _ => "Home"
        };

        App.Navigation.SetSelectedNavigationItem(tag);
    }

    /// <summary>
    /// Navigate to Gallery Detail page with a photo.
    /// </summary>
    public void NavigateToDetail(object photo)
    {
        ContentFrame.Navigate(typeof(GalleryDetailPage), photo);
    }
}
