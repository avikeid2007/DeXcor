
using DeXcor.Services;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.Web.Http;

namespace DeXcor.Helpers
{
    public static class BackgroundHelper
    {
        public static async Task SetBackgroundAsync(Photo selectedPhoto, bool IsCheckPrevious = true)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(selectedPhoto.source.original));
                if (response != null && response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                {
                    var type = response.Content.Headers.ContentType;
                    string filename = "background.jpg";
                    var imageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    using (IRandomAccessStream stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await response.Content.WriteToStreamAsync(stream);
                    }
                    StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                    if (await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file) && IsCheckPrevious)
                    {
                        (ImageDataService.UsedList ?? (ImageDataService.UsedList = new List<Photo>())).Add(selectedPhoto);
                    }
                }
            }
        }
    }
}
