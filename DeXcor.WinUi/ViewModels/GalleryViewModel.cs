using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DeXcor.WinUi.Services;

using PexelsDotNetSDK.Models;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DeXcor.WinUi.ViewModels;

/// <summary>
/// ViewModel for the gallery page.
/// </summary>
public partial class GalleryViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _selectedCategoryIndex;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string? _selectedOrientation;

    [ObservableProperty]
    private string? _selectedColor;

    [ObservableProperty]
    private string? _selectedSize;

    [ObservableProperty]
    private Photo? _selectedPhoto;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    public ObservableCollection<Photo> Photos { get; } = [];

    public string[] Categories { get; } = PhotoCategories.All.Select(c => $"{c.Emoji} {c.Name}").ToArray();
    public string[] Orientations { get; } = ["Any", "Landscape", "Portrait", "Square"];
    public string[] Sizes { get; } = ["Any", "Large", "Medium", "Small"];

    public GalleryViewModel()
    {
        SelectedCategoryIndex = 0;
    }

    public async Task InitializeAsync()
    {
        if (Photos.Count == 0)
        {
            await LoadPhotosAsync();
        }
    }

    partial void OnSelectedCategoryIndexChanged(int value)
    {
        CurrentPage = 1;
        _ = LoadPhotosAsync();
    }

    [RelayCommand]
    private async Task LoadPhotosAsync()
    {
        if (!App.Pexels.IsInitialized) return;

        try
        {
            IsLoading = true;
            Photos.Clear();

            var category = PhotoCategories.All[SelectedCategoryIndex];
            PhotoPage? result;

            if (category.Name == "Search" && !string.IsNullOrWhiteSpace(SearchQuery))
            {
                result = await App.Pexels.SearchPhotosAdvancedAsync(
                    SearchQuery,
                    orientation: GetOrientationParam(),
                    size: GetSizeParam(),
                    color: SelectedColor,
                    page: CurrentPage);
            }
            else if (category.Query is not null)
            {
                result = await App.Pexels.SearchPhotosAdvancedAsync(
                    category.Query,
                    orientation: GetOrientationParam(),
                    size: GetSizeParam(),
                    color: SelectedColor,
                    page: CurrentPage);
            }
            else
            {
                result = await App.Pexels.GetCuratedPhotosAsync(CurrentPage);
            }

            if (result?.photos is not null)
            {
                foreach (var photo in result.photos)
                {
                    Photos.Add(photo);
                }
                TotalPages = (int)Math.Ceiling(result.totalResults / 40.0);
            }
        }
        catch (Exception)
        {
            // Handle error
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        SelectedCategoryIndex = Array.FindIndex(PhotoCategories.All, c => c.Name == "Search");
        CurrentPage = 1;
        await LoadPhotosAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadPhotosAsync();
        }
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadPhotosAsync();
        }
    }

    [RelayCommand]
    private async Task QuickSetWallpaperAsync(Photo photo)
    {
        await App.Wallpaper.SetWallpaperAsync(photo);
    }

    [RelayCommand]
    private async Task QuickDownloadAsync(Photo photo)
    {
        var category = PhotoCategories.All[SelectedCategoryIndex].Name;
        await App.Downloads.EnqueueAsync(photo, "original", category);
    }

    private string? GetOrientationParam() => SelectedOrientation?.ToLowerInvariant() switch
    {
        "landscape" => "landscape",
        "portrait" => "portrait",
        "square" => "square",
        _ => null
    };

    private string? GetSizeParam() => SelectedSize?.ToLowerInvariant() switch
    {
        "large" => "large",
        "medium" => "medium",
        "small" => "small",
        _ => null
    };
}
