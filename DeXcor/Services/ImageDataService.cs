using DeXcor.Helpers;
using DeXcor.Models;

using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Popups;

namespace DeXcor.Services
{
    public static class ImageDataService
    {
        public const string GallerySelectedIdKey = "GallerySelectedIdKey";
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
                return PhotoCatalogCollection.Select(x => x.PhotoType);
            }
        }
        public static IEnumerable<PhotoCatalog> PhotoCatalogCollection
        {
            get
            {
                yield return new PhotoCatalog { PhotoType = "Curated", Emoji = "✨" };
                yield return new PhotoCatalog { PhotoType = "Landscapes", Emoji = "🖼" };
                yield return new PhotoCatalog { PhotoType = "Technology", Emoji = "🐱‍💻" };
                yield return new PhotoCatalog { PhotoType = "Nature", Emoji = "🏞" };
                yield return new PhotoCatalog { PhotoType = "Animals", Emoji = "🦙" };
                yield return new PhotoCatalog { PhotoType = "lifestyle", Emoji = "🗽" };
                yield return new PhotoCatalog { PhotoType = "Search", Emoji = "🔍" };
            }
        }
        public static async Task FetchHomeWallPaperListAsync(int page = 1, string keyword = "Landscapes")
        {
            try
            {
                if (!string.IsNullOrEmpty(ApiKey))
                {
                    var pexelsClient = new PexelsClient(ApiKey);
                    photoPage = await pexelsClient.SearchPhotosAsync(keyword, page: page, pageSize: 80);
                    CuratedWallpaperCollection = photoPage.photos.OrderBy(x => Guid.NewGuid()).ToList();

                }
            }
            catch (Exception ex)
            {
                Telemetry.LogException(ex);
                await new MessageDialog("Ops, Something went wrong.").ShowAsync();
            }
        }
        public static async Task FillHomeWallpaperList()
        {
            string type = await ApplicationData.Current.LocalSettings.ReadAsync<string>("PhotoType");

            if (string.IsNullOrEmpty(type))
            {
                await ImageDataService.FetchHomeWallPaperListAsync(new Random().Next(1, 5));
            }
            else
            {
                await ImageDataService.FetchHomeWallPaperListAsync(new Random().Next(1, 5), type);
            }
        }
        public static async Task<PhotoPage> FetchCuratedListAsync(int page = 1)
        {
            var pexelsClient = new PexelsClient(ApiKey);
            return await pexelsClient.CuratedPhotosAsync(page: page, pageSize: 80);
        }
        public static async Task<PhotoPage> FetchWallpaperListAsync(string keyword, int page = 1)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return null;
            }
            var pexelsClient = new PexelsClient(ApiKey);
            return await pexelsClient.SearchPhotosAsync(keyword, page: page, pageSize: 80);
        }
    }

}
