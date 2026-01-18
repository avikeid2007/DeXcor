using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace DeXcor.WinUi.Services;

/// <summary>
/// Service for setting video wallpapers using Lively Wallpaper.
/// </summary>
public class VideoWallpaperService
{
    private const string LivelyProcessName = "Lively.UI.WinUI";
    private const string LivelyCommandLine = "livelywpf://";

    private readonly HttpClient _httpClient = new();

    /// <summary>
    /// Check if Lively Wallpaper is installed.
    /// </summary>
    public bool IsLivelyInstalled()
    {
        // Check common installation paths
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var livelyPaths = new[]
        {
            Path.Combine(appData, "Lively Wallpaper"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Lively Wallpaper"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Lively Wallpaper")
        };

        foreach (var path in livelyPaths)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
        }

        // Check if process is running
        return Process.GetProcessesByName(LivelyProcessName).Length > 0;
    }

    /// <summary>
    /// Check if Lively is currently running.
    /// </summary>
    public bool IsLivelyRunning()
    {
        return Process.GetProcessesByName(LivelyProcessName).Length > 0 ||
               Process.GetProcessesByName("Lively").Length > 0;
    }

    /// <summary>
    /// Set a local video file as wallpaper using Lively.
    /// </summary>
    public async Task<bool> SetVideoWallpaperAsync(string videoPath)
    {
        if (!File.Exists(videoPath))
        {
            return false;
        }

        try
        {
            // Use Lively's command line interface
            var startInfo = new ProcessStartInfo
            {
                FileName = $"{LivelyCommandLine}setwallpaper?file={Uri.EscapeDataString(videoPath)}",
                UseShellExecute = true
            };

            Process.Start(startInfo);
            await Task.Delay(500); // Give Lively time to process
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Download a video from URL and set as wallpaper.
    /// </summary>
    public async Task<bool> SetVideoWallpaperFromUrlAsync(string url, string? filename = null)
    {
        try
        {
            // Download video to temp folder
            var tempPath = Path.Combine(Path.GetTempPath(), "DeXcor");
            Directory.CreateDirectory(tempPath);

            filename ??= $"video_{DateTime.Now:yyyyMMddHHmmss}.mp4";
            var localPath = Path.Combine(tempPath, filename);

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var fileStream = new FileStream(localPath, FileMode.Create);
            await response.Content.CopyToAsync(fileStream);

            return await SetVideoWallpaperAsync(localPath);
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Set a video from Pexels as wallpaper.
    /// </summary>
    public async Task<bool> SetPexelsVideoWallpaperAsync(PexelsDotNetSDK.Models.Video video)
    {
        // Get best quality video file
        var videoFile = video.videoFiles
            .OrderByDescending(v => v.width * v.height)
            .FirstOrDefault();

        if (videoFile?.link is null)
        {
            return false;
        }

        return await SetVideoWallpaperFromUrlAsync(
            videoFile.link,
            $"pexels_video_{video.id}.mp4");
    }

    /// <summary>
    /// Open Lively Wallpaper store page.
    /// </summary>
    public async Task OpenLivelyStorePageAsync()
    {
        try
        {
            var uri = new Uri("ms-windows-store://pdp/?ProductId=9NTM2QC6QWS7");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
        catch (Exception)
        {
            // Fallback to web
            var uri = new Uri("https://www.microsoft.com/store/apps/9NTM2QC6QWS7");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
