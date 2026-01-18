using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeXcor.WinUi.Services;
using PexelsDotNetSDK.Models;
using System;
using System.Threading.Tasks;

namespace DeXcor.WinUi.ViewModels;

public partial class CollectionItemViewModel : ObservableObject
{
    public Collection Model { get; }

    [ObservableProperty]
    private string? _photo1Url;

    [ObservableProperty]
    private string? _photo2Url;

    [ObservableProperty]
    private string? _photo3Url;

    [ObservableProperty]
    private string? _photo4Url;

    public CollectionItemViewModel(Collection model)
    {
        Model = model;
    }

    public async Task LoadPreviewsAsync()
    {
        try
        {
            var mediaPage = await App.Pexels.GetCollectionMediaAsync(Model.id, pageSize: 6);
            if (mediaPage?.media != null)
            {
                int count = 0;
                foreach (var item in mediaPage.media)
                {
                    try
                    {
                        string? url = null;
                        dynamic dynItem = item;

                        // Try different property paths for image URL
                        try { url = dynItem.source?.large; } catch { }
                        if (string.IsNullOrEmpty(url)) try { url = dynItem.source?.medium; } catch { }
                        if (string.IsNullOrEmpty(url)) try { url = dynItem.src?.large; } catch { }
                        if (string.IsNullOrEmpty(url)) try { url = dynItem.src?.medium; } catch { }
                        if (string.IsNullOrEmpty(url)) try { url = dynItem.image; } catch { } // Video thumbnail

                        if (!string.IsNullOrEmpty(url))
                        {
                            switch (count)
                            {
                                case 0: Photo1Url = url; break;
                                case 1: Photo2Url = url; break;
                                case 2: Photo3Url = url; break;
                                case 3: Photo4Url = url; break;
                            }
                            count++;
                            if (count >= 4) break;
                        }
                    }
                    catch { }
                }
            }
        }
        catch { }
    }
}
