using DeXcor.Behaviors;
using DeXcor.Helpers;
using DeXcor.Services;
using Microsoft.Toolkit.Uwp.UI.Animations;
using PexelsDotNetSDK.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace DeXcor.Views
{
    public sealed partial class GalleryPage : Windows.UI.Xaml.Controls.Page, INotifyPropertyChanged
    {
        public const string GallerySelectedIdKey = "GallerySelectedIdKey";
        private string searchText;

        public ObservableCollection<Photo> Source { get; set; }

        public GalleryPage()
        {
            InitializeComponent();
            Loaded += GalleryPage_OnLoaded;

        }
        private void GalleryPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["SearchHeader"] as DataTemplate);
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

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Pivot pv && pv.SelectedItem is string selected)
            {
                if (selected.Equals("Curated", System.StringComparison.OrdinalIgnoreCase))
                {
                    var page = await ImageDataService.FetchCuratedListAsync();
                    FillGridView(page);
                }
                else
                {
                    await SearchWallpapersAsync(selected);
                }

            }
        }

        private async Task SearchWallpapersAsync(string selected)
        {
            var page = await ImageDataService.FetchWallpaperListAsync(keyword: selected);
            FillGridView(page);
        }

        private void FillGridView(PhotoPage page)
        {
            ImageDataService.ImageCollection = page.photos;
            ImgGridView.ItemsSource = ImageDataService.ImageCollection;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                FillGridView(await ImageDataService.FetchWallpaperListAsync(searchText));
            }
        }

        private async void TextBox_KeyDownAsync(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                FillGridView(await ImageDataService.FetchWallpaperListAsync(textBox.Text));
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                searchText = textBox.Text;
            }
        }
    }
}
