using DeXcor.Behaviors;
using DeXcor.Helpers;
using DeXcor.Services;
using DeXcor.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace DeXcor.Views
{
    public sealed partial class GalleryDetailPage : Page
    {
        public GalleryDetailViewModel ViewModel { get; set; }
        public GalleryDetailPage()
        {
            InitializeComponent();
            ViewModel = new GalleryDetailViewModel();
            this.DataContext = ViewModel;
            Loaded += GalleryPage_OnLoaded;
        }
        private void GalleryPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["MenuHeader"] as DataTemplate);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int selectedImageID && e.NavigationMode == NavigationMode.New)
            {
                ViewModel.SelectedImage = ViewModel.Source.FirstOrDefault(i => i.id == selectedImageID);
            }
            else
            {
                selectedImageID = ImagesNavigationHelper.GetImageId(ImageDataService.GallerySelectedIdKey);
                if (selectedImageID != 0)
                {
                    ViewModel.SelectedImage = ViewModel.Source.FirstOrDefault(i => i.id == selectedImageID);
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(ViewModel.SelectedImage);
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
    }
}
