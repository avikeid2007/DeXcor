using DeXcor.Behaviors;
using DeXcor.ViewModels;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DeXcor.Views
{
    public sealed partial class MainPage : Page
    {
        private bool IsCompactMode;

        public MainViewModel ViewModel { get; set; }
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["SearchHeader"] as DataTemplate);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsCompactMode)
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                IsCompactMode = false;
            }
            else
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                IsCompactMode = true;
            }

        }
    }
}
