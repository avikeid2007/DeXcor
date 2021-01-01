using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                yield return "Curated";
                yield return "Landscapes";
                yield return "Technology";
                yield return "Nature";
                yield return "Animals";
                yield return "lifestyle";
                yield return "Search";
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
