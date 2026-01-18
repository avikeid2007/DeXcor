using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeXcor.WinUi.Services;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DeXcor.WinUi.ViewModels;



public partial class CollectionsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<CollectionItemViewModel> Collections { get; } = new();

    public CollectionsViewModel()
    {
        // Loading is triggered by OnNavigatedTo in CollectionsPage
    }

    [RelayCommand]
    private async Task LoadCollectionsAsync()
    {
        if (IsLoading) return;
        IsLoading = true;

        try
        {
            var result = await App.Pexels.GetFeaturedCollectionsAsync();
            foreach (var col in result.collections)
            {
                var vm = new CollectionItemViewModel(col);
                Collections.Add(vm);
                // Fire and forget loading previews to not block UI
                _ = vm.LoadPreviewsAsync();
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
}
