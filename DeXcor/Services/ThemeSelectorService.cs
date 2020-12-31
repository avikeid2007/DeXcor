using DeXcor.Helpers;
using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace DeXcor.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme { get; set; } = ElementTheme.Default;

        public static async Task InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();
            await SaveThemeInSettingsAsync(Theme);
        }

        public static async Task SetRequestedThemeAsync()
        {
            foreach (var view in CoreApplication.Views)
            {
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = Theme;
                    }
                });
            }
        }

        private static async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private static async Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, theme.ToString());
        }
    }
    public static class ImageDataService
    {
        public static string ApiKey { get; set; }
        public static PhotoPage photoPage { get; set; }
        public static List<Photo> ImageCollection { get; set; }
        public static List<Photo> CuratedWallpaperCollection { get; set; }
        public static List<Photo> UsedList;
        public static string SearchText { get; set; }
        public static IEnumerable<string> PhotoType
        {
            get
            {
                yield return "Curated";
                yield return "Landscapes";
                yield return "Technology";
                yield return "Nature";
                yield return "Animals";
                yield return "lifestyle";
            }
        }
        public static async Task FetchHomeWallPaperListAsync(int page = 1, string keyword = "Landscapes")
        {
            try
            {
                if (!string.IsNullOrEmpty(ApiKey) && CuratedWallpaperCollection?.Count > 0)
                {
                    var pexelsClient = new PexelsClient(ApiKey);
                    photoPage = await pexelsClient.SearchPhotosAsync(keyword, page: page, pageSize: 80);
                    CuratedWallpaperCollection = photoPage.photos.OrderBy(x => Guid.NewGuid()).ToList();

                }
            }
            catch
            {
                await new MessageDialog("Ops, Something went wrong.").ShowAsync();
            }
        }
        public static async Task<PhotoPage> FetchCuratedListAsync(int page = 1)
        {
            var pexelsClient = new PexelsClient(ApiKey);
            return await pexelsClient.CuratedPhotosAsync(page: page, pageSize: 80);
        }
        public static async Task<PhotoPage> FetchWallpaperListAsync(string keyword, int page = 1)
        {
            var pexelsClient = new PexelsClient(ApiKey);
            return await pexelsClient.SearchPhotosAsync(keyword, page: page, pageSize: 80);
        }
    }
}
