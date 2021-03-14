using BasicMvvm;
using DeXcor.Helpers;
using DeXcor.Services;
using DeXcor.Views;
using Microsoft.Toolkit.Uwp.UI.Animations;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DeXcor.ViewModels
{
    public class GalleryViewModel : BindableBase
    {
        private ObservableCollection<Photo> _source;
        private Photo _selectedPhoto;
        private string _selectedPivot;
        private string _selectedPage;
        public string SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                if (!string.IsNullOrEmpty(value) && currentPage != null)
                {
                    _ = SetPageDataAsync(value);
                }
                else
                {
                    ImageDataService.ImageCollection = null;
                    Source = null;
                }
                OnPropertyChanged();
            }
        }

        private async Task SetPageDataAsync(string value)
        {
            needPagination = false;
            if (Convert.ToInt32(value) != currentPage.page)
            {
                await SetWallpaperListAsync(SelectedPivot, Convert.ToInt32(value));
            }
            SetCollection();
        }

        private void SetCollection()
        {
            ImageDataService.ImageCollection = currentPage.photos;
            Source = new ObservableCollection<Photo>(ImageDataService.ImageCollection);
        }

        public ObservableCollection<Photo> Source
        {
            get { return _source; }
            set
            {
                _source = value;
                OnPropertyChanged();
            }
        }
        public Photo SelectedPhoto
        {
            get { return _selectedPhoto; }
            set
            {
                _selectedPhoto = value;
                NavToFullScreen(value);
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _totalPages;
        private PhotoPage currentPage;
        private bool needPagination;

        public ObservableCollection<string> TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }
        private void NavToFullScreen(Photo selected)
        {
            if (selected != null)
            {
                ImagesNavigationHelper.AddImageId(ImageDataService.GallerySelectedIdKey, selected.id);
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(selected);
                NavigationService.Navigate<GalleryDetailPage>(selected.id);
            }
        }

        public string SearchText
        {
            get;
            set;
        }
        public string SelectedPivot
        {
            get { return _selectedPivot; }
            set
            {
                if (value != null)
                {
                    _selectedPivot = value;
                    needPagination = true;
                    SelectedPage = string.Empty;
                    TotalPages = null;
                    OnPropertyChanged("SelectedPivot");
                    _ = SetWallpaperListAsync(value);
                }
            }
        }

        private async Task SetWallpaperListAsync(string selected, int pageCount = 1)
        {
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.Equals("Curated", System.StringComparison.OrdinalIgnoreCase))
                {
                    var page = await ImageDataService.FetchCuratedListAsync(pageCount);
                    FillGridView(page);
                }
                else
                {
                    if (selected.Equals("Search", System.StringComparison.OrdinalIgnoreCase))
                    {
                        await SearchWallpapersAsync(SearchText, pageCount);
                    }
                    else
                        await SearchWallpapersAsync(selected, pageCount);
                }
            }
        }
        public void FillGridView(PhotoPage page)
        {
            if (page != null)
            {
                currentPage = page;
                if (needPagination)
                    SetPagination(page);

            }
        }
        public void Search()
        {
            needPagination = true;

            SelectedPivot = ImageDataService.PhotoType.FirstOrDefault(x => x.Equals("Search", StringComparison.OrdinalIgnoreCase));
        }
        private void SetPagination(PhotoPage page)
        {
            var pageCount = Math.Ceiling((float)page.totalResults / (float)page.perPage);
            TotalPages = new ObservableCollection<string>(SetPaginationCount(pageCount));
            if (TotalPages?.Count > 0)
                SelectedPage = TotalPages.First();

        }
        private IEnumerable<string> SetPaginationCount(double pageCount)
        {
            if (pageCount > 0)
            {
                for (int i = 0; i < pageCount; i++)
                {
                    yield return Convert.ToString(i + 1);
                }
            }
        }
        private async Task SearchWallpapersAsync(string selected, int pageCount = 1)
        {
            var page = await ImageDataService.FetchWallpaperListAsync(keyword: selected, pageCount);
            FillGridView(page);
        }

    }
}
