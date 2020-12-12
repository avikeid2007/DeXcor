using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using DeXcor.Core.Models;
using DeXcor.Core.Services;
using DeXcor.Helpers;
using DeXcor.Services;

using Microsoft.Toolkit.Uwp.UI.Animations;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DeXcor.Views
{
    public sealed partial class GalleryPage : Page, INotifyPropertyChanged
    {
        public const string GallerySelectedIdKey = "GallerySelectedIdKey";

        public ObservableCollection<SampleImage> Source { get; } = new ObservableCollection<SampleImage>();

        public GalleryPage()
        {
            InitializeComponent();
            Loaded += GalleryPage_OnLoaded;
        }

        private async void GalleryPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Source.Clear();

            // TODO WTS: Replace this with your actual data
            var data = await SampleDataService.GetImageGalleryDataAsync("ms-appx:///Assets");

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        private void ImagesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as SampleImage;
            ImagesNavigationHelper.AddImageId(GallerySelectedIdKey, selected.ID);
            NavigationService.Frame.SetListDataItemForNextConnectedAnimation(selected);
            NavigationService.Navigate<GalleryDetailPage>(selected.ID);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
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
