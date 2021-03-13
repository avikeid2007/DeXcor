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

            return await openPicker.PickSingleFileAsync();
        }
        public static async Task<bool> DownloadImageAsync(string url, string fileName = "")
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation =
                PickerLocationId.PicturesLibrary
            };
            savePicker.FileTypeChoices.Add("Image File", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = fileName;
            StorageFile file = await savePicker.PickSaveFileAsync();
            return await DownloadFileFromURLAsync(file, url);

        }
        public static async Task<bool> DownloadFileFromURLAsync(StorageFile file, string url)
        {
            if (file != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(new Uri(url));
                    if (response?.StatusCode == HttpStatusCode.Ok)
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
