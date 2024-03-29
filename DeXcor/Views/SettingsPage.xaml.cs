﻿using DeXcor.Helpers;
using DeXcor.Models;
using DeXcor.Services;

using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DeXcor.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;
        private bool isTypeChanged;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
            var homeIndex = ImageDataService.PhotoCatalogCollection.ToList().FindIndex(x => x.PhotoType.Equals("Search", System.StringComparison.OrdinalIgnoreCase));
            var collection = ImageDataService.PhotoCatalogCollection.ToList();
            collection.RemoveAt(homeIndex);
            cbPhoto.ItemsSource = collection;
            string type = await ApplicationData.Current.LocalSettings.ReadAsync<string>("PhotoType");
            if (!string.IsNullOrEmpty(type))
            {
                var item = ImageDataService.PhotoCatalogCollection.ToList().FindIndex(x => x.PhotoType == type);
                if (item >= 0)
                {
                    cbPhoto.SelectedIndex = item;
                }
                else
                {
                    cbPhoto.SelectedIndex = homeIndex;
                    photoText.Text = type;
                }
            }

        }
        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (isTypeChanged)
            {
                await ImageDataService.FillHomeWallpaperList();
            }
            base.OnNavigatingFrom(e);
        }
        private async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
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

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb && cb.SelectedItem is PhotoCatalog type && !string.IsNullOrEmpty(type?.PhotoType))
            {
                if (type.PhotoType.Equals("Search", System.StringComparison.OrdinalIgnoreCase))
                {
                    photoText.Visibility = Visibility.Visible;
                    await ApplicationData.Current.LocalSettings.SaveAsync("PhotoType", string.Empty);
                }
                else
                {
                    photoText.Visibility = Visibility.Collapsed;
                    await ApplicationData.Current.LocalSettings.SaveAsync("PhotoType", type.PhotoType);
                    isTypeChanged = true;

                }
            }
        }

        private async void photoText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb && !string.IsNullOrEmpty(tb.Text))
            {
                await ApplicationData.Current.LocalSettings.SaveAsync("PhotoType", tb.Text);
            }
        }
    }
}
