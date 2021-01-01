using DeXcor.Behaviors;
using DeXcor.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace DeXcor.Views
{
    public sealed partial class GalleryPage : Windows.UI.Xaml.Controls.Page, INotifyPropertyChanged
    {

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                ViewModel.SearchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        public GalleryViewModel ViewModel;
        public GalleryPage()
        {
            InitializeComponent();
            ViewModel = new GalleryViewModel();
            this.DataContext = ViewModel;
            Loaded += GalleryPage_OnLoaded;
        }
        private void GalleryPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["SearchHeader"] as DataTemplate);
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


        private async void AppBarButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                ViewModel.Search();
            }
        }

        private async void TextBox_KeyDownAsync(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(SearchText))
            {
                ViewModel.Search();
            }
        }
    }
}
