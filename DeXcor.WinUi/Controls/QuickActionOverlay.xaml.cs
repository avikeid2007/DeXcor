using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PexelsDotNetSDK.Models;

namespace DeXcor.WinUi.Controls;

/// <summary>
/// Quick action overlay for gallery thumbnails.
/// </summary>
public sealed partial class QuickActionOverlay : UserControl
{
    public event EventHandler<Photo>? WallpaperRequested;
    public event EventHandler<Photo>? DownloadRequested;
    public event EventHandler<Photo>? DetailRequested;

    public Photo? Photo { get; set; }

    public QuickActionOverlay()
    {
        this.InitializeComponent();
    }

    private void SetWallpaper_Click(object sender, RoutedEventArgs e)
    {
        if (Photo is not null)
        {
            WallpaperRequested?.Invoke(this, Photo);
        }
    }

    private void Download_Click(object sender, RoutedEventArgs e)
    {
        if (Photo is not null)
        {
            DownloadRequested?.Invoke(this, Photo);
        }
    }

    private void OpenDetail_Click(object sender, RoutedEventArgs e)
    {
        if (Photo is not null)
        {
            DetailRequested?.Invoke(this, Photo);
        }
    }
}
