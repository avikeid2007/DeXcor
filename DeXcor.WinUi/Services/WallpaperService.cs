using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using PexelsDotNetSDK.Models;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;

namespace DeXcor.WinUi.Services;

/// <summary>
/// Service for setting desktop wallpapers.
/// </summary>
public class WallpaperService
{
    private readonly List<Photo> _usedPhotos = [];

    /// <summary>
    /// Set a photo as the desktop wallpaper.
    /// </summary>
    public async Task<bool> SetWallpaperAsync(Photo photo, bool trackUsage = true)
    {
        if (!UserProfilePersonalizationSettings.IsSupported())
        {
            return false;
        }

        try
        {
            // Download the image
            var imageFile = await DownloadImageAsync(photo.source.original, "wallpaper.jpg");
            if (imageFile is null) return false;

            // Set as wallpaper
            var success = await UserProfilePersonalizationSettings.Current
                .TrySetWallpaperImageAsync(imageFile);

            if (success && trackUsage)
            {
                _usedPhotos.Add(photo);
            }

            return success;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Set an image file as the desktop wallpaper.
    /// </summary>
    public async Task<bool> SetWallpaperFromFileAsync(StorageFile file)
    {
        if (!UserProfilePersonalizationSettings.IsSupported())
        {
            return false;
        }

        try
        {
            return await UserProfilePersonalizationSettings.Current
                .TrySetWallpaperImageAsync(file);
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Download an image from URL to local storage.
    /// </summary>
    public async Task<StorageFile?> DownloadImageAsync(string url, string filename)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return null;

            var imageFile = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using var stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite);
            var content = await response.Content.ReadAsByteArrayAsync();
            await stream.WriteAsync(content.AsBuffer());

            return imageFile;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Get a random unused photo from a collection.
    /// </summary>
    public Photo? GetRandomUnusedPhoto(IEnumerable<Photo> photos)
    {
        var unused = photos.Where(p => !_usedPhotos.Contains(p)).ToList();
        if (unused.Count == 0) return null;

        var random = new Random();
        return unused[random.Next(unused.Count)];
    }

    /// <summary>
    /// Clear the usage history.
    /// </summary>
    public void ClearUsageHistory()
    {
        _usedPhotos.Clear();
    }

    /// <summary>
    /// Check if wallpaper setting is supported.
    /// </summary>
    public static bool IsSupported => UserProfilePersonalizationSettings.IsSupported();
}
