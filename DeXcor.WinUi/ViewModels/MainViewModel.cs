using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PexelsDotNetSDK.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DeXcor.WinUi.ViewModels;

/// <summary>
/// ViewModel for the main/home page.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasSetWallpaper;

    [ObservableProperty]
    private Photo? _currentPhoto;

    [ObservableProperty]
    private string _statusMessage = "Click to set a random wallpaper!";

    [ObservableProperty]
    private bool _isApiConfigured;

    [ObservableProperty]
    private ObservableCollection<Photo> _backgroundWallpapers = new();

    private List<Photo>? _wallpaperCollection;

    public MainViewModel()
    {
        IsApiConfigured = App.Pexels.IsInitialized;
    }

    public async Task InitializeAsync()
    {
        IsApiConfigured = App.Pexels.IsInitialized;

        if (IsApiConfigured && _wallpaperCollection is null)
        {
            await LoadWallpaperCollectionAsync();
        }
    }

    private async Task LoadWallpaperCollectionAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading wallpapers...";

            var page = await App.Pexels.GetCuratedPhotosAsync(page: Random.Shared.Next(1, 10), pageSize: 80);
            _wallpaperCollection = page.photos.ToList();

            // Populate background grid (take all to fill screen)
            BackgroundWallpapers.Clear();
            foreach (var p in _wallpaperCollection)
            {
                BackgroundWallpapers.Add(p);
            }

            StatusMessage = "Click to set a random wallpaper!";

            // Set initial featured photo to show the info card
            if (BackgroundWallpapers.Count > 0)
            {
                CurrentPhoto = BackgroundWallpapers[0];
                HasSetWallpaper = true;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SurpriseMeAsync()
    {
        if (!IsApiConfigured)
        {
            StatusMessage = "Please configure your Pexels API key in Settings";
            return;
        }

        if (_wallpaperCollection is null || _wallpaperCollection.Count == 0)
        {
            await LoadWallpaperCollectionAsync();
        }

        if (_wallpaperCollection is null || _wallpaperCollection.Count == 0)
        {
            StatusMessage = "No wallpapers available";
            return;
        }

        try
        {
            IsLoading = true;
            HasSetWallpaper = false;
            StatusMessage = "Finding a beautiful wallpaper...";

            // Get random unused photo
            var photo = App.Wallpaper.GetRandomUnusedPhoto(_wallpaperCollection);

            if (photo is null)
            {
                // All used, fetch new batch
                App.Wallpaper.ClearUsageHistory();
                await LoadWallpaperCollectionAsync();
                photo = App.Wallpaper.GetRandomUnusedPhoto(_wallpaperCollection!);
            }

            if (photo is null)
            {
                StatusMessage = "No wallpapers available";
                return;
            }

            CurrentPhoto = photo;
            StatusMessage = "Setting wallpaper...";

            var success = await App.Wallpaper.SetWallpaperAsync(photo);

            if (success)
            {
                HasSetWallpaper = true;
                StatusMessage = "Wallpaper set successfully!";
            }
            else
            {
                StatusMessage = "Failed to set wallpaper";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        App.Navigation.SetSelectedNavigationItem("Settings");
    }
}
