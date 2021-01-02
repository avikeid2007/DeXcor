using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace DeXcor.Helpers
{
    public static class ImageHelper
    {
        public static async Task<StorageFile> LoadImageFileAsync()
        {
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".bmp");

            var imageFile = await openPicker.PickSingleFileAsync();

            return imageFile;
        }
        public static async Task<bool> DownloadImageAsync(string url, string fileName = "")
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            savePicker.FileTypeChoices.Add("Image File", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = fileName;
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(new Uri(url));
                    if (response != null && response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                    {
                        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await response.Content.WriteToStreamAsync(stream);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public static async Task<BitmapImage> GetBitmapFromImageAsync(StorageFile file)
        {
            if (file == null)
            {
                return null;
            }

            try
            {
                using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    return bitmapImage;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
