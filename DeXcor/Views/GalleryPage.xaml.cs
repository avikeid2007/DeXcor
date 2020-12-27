using DeXcor.Helpers;
using DeXcor.Services;
using Microsoft.Toolkit.Uwp.UI.Animations;
using PexelsDotNetSDK.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DeXcor.Views
{
    public sealed partial class GalleryPage : Windows.UI.Xaml.Controls.Page, INotifyPropertyChanged
    {
        public const string GallerySelectedIdKey = "GallerySelectedIdKey";
        public ObservableCollection<Photo> Source { get; set; }
        public GalleryPage()
        {
            InitializeComponent();
            Loaded += GalleryPage_OnLoaded;

        }
        private void GalleryPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            ImageDataService.ImageCollection = new ObservableCollection<Photo>(ImageDataService.CuratedWallpaperCollection);
            ImgGridView.ItemsSource = ImageDataService.ImageCollection;
        }
        private void ImagesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as Photo;
            ImagesNavigationHelper.AddImageId(GallerySelectedIdKey, selected.id);
            NavigationService.Frame.SetListDataItemForNextConnectedAnimation(selected);
            NavigationService.Navigate<GalleryDetailPage>(selected.id);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
