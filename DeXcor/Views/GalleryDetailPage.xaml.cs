using DeXcor.Helpers;
using DeXcor.Services;
using Microsoft.Toolkit.Uwp.UI.Animations;
using PexelsDotNetSDK.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace DeXcor.Views
{
    public sealed partial class GalleryDetailPage : Windows.UI.Xaml.Controls.Page, INotifyPropertyChanged
    {
        private Photo _selectedImage;

        public Photo SelectedImage
        {
            get => _selectedImage;
            set
            {
                Set(ref _selectedImage, value);
                ImagesNavigationHelper.UpdateImageId(ImageDataService.GallerySelectedIdKey, SelectedImage.id);
            }
        }
        public ObservableCollection<Photo> Source { get; } = new ObservableCollection<Photo>(ImageDataService.ImageCollection);

        public GalleryDetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int selectedImageID && e.NavigationMode == NavigationMode.New)
            {
                SelectedImage = Source.FirstOrDefault(i => i.id == selectedImageID);
            }
            else
            {
                selectedImageID = ImagesNavigationHelper.GetImageId(ImageDataService.GallerySelectedIdKey);
                if (selectedImageID != 0)
                {
                    SelectedImage = Source.FirstOrDefault(i => i.id == selectedImageID);
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(SelectedImage);
                ImagesNavigationHelper.RemoveImageId(ImageDataService.GallerySelectedIdKey);
            }
        }

        private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
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
