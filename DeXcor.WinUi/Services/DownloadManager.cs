using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PexelsDotNetSDK.Models;
using Windows.Storage;

namespace DeXcor.WinUi.Services;

/// <summary>
/// Download manager for queuing and tracking image downloads.
/// </summary>
public partial class DownloadManager : ObservableObject
{
    private readonly string _downloadFolder;

    public ObservableCollection<DownloadItem> ActiveDownloads { get; } = [];
    public ObservableCollection<DownloadItem> CompletedDownloads { get; } = [];

    [ObservableProperty]
    private int _totalDownloaded;

    [ObservableProperty]
    private long _totalBytes;

    public DownloadManager()
    {
        _downloadFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "DeXcor");

        Directory.CreateDirectory(_downloadFolder);
    }

    /// <summary>
    /// Queue a photo for download.
    /// </summary>
    public async Task<DownloadItem> EnqueueAsync(Photo photo, string size = "original", string? category = null)
    {
        var item = new DownloadItem
        {
            Photo = photo,
            Size = size,
            Category = category ?? "Uncategorized",
            Status = DownloadStatus.Queued
        };

        ActiveDownloads.Add(item);

        try
        {
            await DownloadAsync(item);
        }
        catch (Exception ex)
        {
            item.Status = DownloadStatus.Failed;
            item.ErrorMessage = ex.Message;
        }

        return item;
    }

    private async Task DownloadAsync(DownloadItem item)
    {
        item.Status = DownloadStatus.Downloading;

        var url = GetUrlForSize(item.Photo, item.Size);
        var categoryFolder = Path.Combine(_downloadFolder, item.Category);
        Directory.CreateDirectory(categoryFolder);

        var filename = $"{item.Photo.id}_{item.Size}.jpg";
        var filePath = Path.Combine(categoryFolder, filename);

        using var httpClient = new HttpClient();

        // Get content length for progress
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        item.TotalBytes = totalBytes;

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var buffer = new byte[8192];
        long downloadedBytes = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            downloadedBytes += bytesRead;
            item.DownloadedBytes = downloadedBytes;
            item.Progress = totalBytes > 0 ? (double)downloadedBytes / totalBytes * 100 : 0;
        }

        item.LocalPath = filePath;
        item.Status = DownloadStatus.Completed;
        item.CompletedAt = DateTime.Now;

        ActiveDownloads.Remove(item);
        CompletedDownloads.Insert(0, item);

        TotalDownloaded++;
        TotalBytes += totalBytes;
    }

    private static string GetUrlForSize(Photo photo, string size)
    {
        return size.ToLowerInvariant() switch
        {
            "original" => photo.source.original,
            "large2x" => photo.source.large2x,
            "large" => photo.source.large,
            "medium" => photo.source.medium,
            "small" => photo.source.small,
            "portrait" => photo.source.portrait,
            "landscape" => photo.source.landscape,
            "tiny" => photo.source.tiny,
            _ => photo.source.original
        };
    }

    /// <summary>
    /// Cancel a download.
    /// </summary>
    public void Cancel(DownloadItem item)
    {
        item.Status = DownloadStatus.Cancelled;
        ActiveDownloads.Remove(item);
    }

    /// <summary>
    /// Retry a failed download.
    /// </summary>
    public async Task RetryAsync(DownloadItem item)
    {
        if (item.Status is DownloadStatus.Failed or DownloadStatus.Cancelled)
        {
            CompletedDownloads.Remove(item);
            item.Progress = 0;
            item.DownloadedBytes = 0;
            await EnqueueAsync(item.Photo, item.Size, item.Category);
        }
    }

    /// <summary>
    /// Open the downloads folder.
    /// </summary>
    public async Task OpenDownloadsFolderAsync()
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(_downloadFolder);
        await Windows.System.Launcher.LaunchFolderAsync(folder);
    }
}

/// <summary>
/// Represents a single download item.
/// </summary>
public partial class DownloadItem : ObservableObject
{
    public required Photo Photo { get; init; }
    public required string Size { get; init; }
    public required string Category { get; set; }

    [ObservableProperty]
    private DownloadStatus _status;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private long _downloadedBytes;

    [ObservableProperty]
    private long _totalBytes;

    [ObservableProperty]
    private string? _localPath;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private DateTime? _completedAt;

    public string DisplaySize => FormatBytes(TotalBytes);

    private static string FormatBytes(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}

public enum DownloadStatus
{
    Queued,
    Downloading,
    Completed,
    Failed,
    Cancelled
}
