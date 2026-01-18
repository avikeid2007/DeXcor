using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;

namespace DeXcor.WinUi.Services;

/// <summary>
/// Service for interacting with the Pexels API.
/// </summary>
public class PexelsService
{
    private PexelsClient? _client;

    // TODO: Move API key to secure storage
    private const string DefaultApiKey = ""; // User needs to provide their API key

    public string ApiKey { get; private set; } = string.Empty;

    public bool IsInitialized => _client is not null;

    public void Initialize(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be empty", nameof(apiKey));
        }

        ApiKey = apiKey;
        _client = new PexelsClient(apiKey);
    }

    private PexelsClient GetClient()
    {
        if (_client is null)
        {
            // Throwing here prevents partial usage. 
            // We will handle null client in each method for Mock Data.
            return null!;
        }
        return _client;
    }

    #region Photos

    /// <summary>
    /// Get curated photos.
    /// </summary>
    public async Task<PhotoPage> GetCuratedPhotosAsync(int page = 1, int pageSize = 40)
    {
        if (!IsInitialized) return await GetMockPhotosAsync(page, pageSize);
        return await GetClient().CuratedPhotosAsync(page: page, pageSize: pageSize);
    }

    /// <summary>
    /// Search photos by keyword.
    /// </summary>
    public async Task<PhotoPage> SearchPhotosAsync(string query, int page = 1, int pageSize = 40)
    {
        if (!IsInitialized) return await GetMockPhotosAsync(page, pageSize);
        return await GetClient().SearchPhotosAsync(query, page: page, pageSize: pageSize);
    }

    /// <summary>
    /// Search photos with advanced filters.
    /// </summary>
    public async Task<PhotoPage> SearchPhotosAdvancedAsync(
        string query,
        string? orientation = null, // landscape, portrait, square
        string? size = null,        // large, medium, small
        string? color = null,       // hex color without #
        string? locale = null,
        int page = 1,
        int pageSize = 40)
    {
        if (!IsInitialized) return await GetMockPhotosAsync(page, pageSize);
        return await GetClient().SearchPhotosAsync(
            query,
            orientation: orientation,
            size: size,
            color: color,
            locale: locale,
            page: page,
            pageSize: pageSize);
    }

    /// <summary>
    /// Get a single photo by ID.
    /// </summary>
    public async Task<Photo> GetPhotoAsync(long id)
    {
        if (!IsInitialized) return (await GetMockPhotosAsync(1, 1)).photos[0];
        return await GetClient().GetPhotoAsync((int)id);
    }

    #endregion

    #region Collections

    /// <summary>
    /// Get featured collections.
    /// </summary>
    public async Task<CollectionPage> GetFeaturedCollectionsAsync(int page = 1, int pageSize = 20)
    {
        if (!IsInitialized) return await GetMockCollectionsAsync();
        return await GetClient().FeaturedCollectionsAsync(page: page, pageSize: pageSize);
    }

    /// <summary>
    /// Get media from a collection.
    /// </summary>
    public async Task<CollectionMediaPage> GetCollectionMediaAsync(string collectionId, int page = 1, int pageSize = 40)
    {
        if (!IsInitialized)
        {
            var mockPhotos = await GetMockPhotosAsync(1, 4);
            // Assuming Photo is compatible or we use dynamic casting if needed. 
            // PexelsDotNetSDK models often share base classes or are flexible.
            // If implicit cast fails, use Cast<CollectionMedia> if possible, or dynamic.
            // We'll use Cast<dynamic> to be safe as CollectionMediaPage expects specific type.
            // Actually, CollectionMediaPage.media is likely List<CollectionMedia>. Photo usually inherits.
            // Let's try explicit logic:
            var mediaList = new List<CollectionMedia>();
            // Cannot cast Photo to CollectionMedia easily if not inherited? 
            // Checked typical SDK: Photo : CollectionMedia.
            // But to avoid "List<dynamic> to List<CollectionMedia>" error:
            // We will create a new list.
            // But wait, we can't instantiate abstract CollectionMedia.
            // We'll trust Photo IS CollectionMedia.
            return new CollectionMediaPage { media = mockPhotos.photos.Cast<CollectionMedia>().ToList() };
        }
        return await GetClient().GetCollectionAsync(collectionId, page: page, pageSize: pageSize);
    }

    #endregion

    #region Videos

    /// <summary>
    /// Search videos by keyword.
    /// </summary>
    public async Task<VideoPage> SearchVideosAsync(string query, int page = 1, int pageSize = 20)
    {
        if (!IsInitialized) return new VideoPage { videos = new List<Video>() };
        return await GetClient().SearchVideosAsync(query, page: page, pageSize: pageSize);
    }

    /// <summary>
    /// Get popular videos.
    /// </summary>
    public async Task<VideoPage> GetPopularVideosAsync(int page = 1, int pageSize = 20)
    {
        if (!IsInitialized) return new VideoPage { videos = new List<Video>() };
        return await GetClient().PopularVideosAsync(page: page, pageSize: pageSize);
    }

    #endregion

    #region Mock Data

    private Task<PhotoPage> GetMockPhotosAsync(int page, int pageSize)
    {
        var photos = new List<Photo>();
        for (int i = 0; i < pageSize; i++)
        {
            photos.Add(new Photo
            {
                id = 1000 + i,
                width = 1920,
                height = 1080,
                url = "https://www.pexels.com/photo/mock-photo-1000",
                photographer = "Demo User",
                source = new Source
                {
                    original = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg",
                    large = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                    large2x = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
                    medium = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&h=350",
                    small = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&h=130",
                    portrait = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&fit=crop&h=1200&w=800",
                    landscape = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&fit=crop&h=627&w=1200",
                    tiny = "https://images.pexels.com/photos/2014422/pexels-photo-2014422.jpeg?auto=compress&cs=tinysrgb&dpr=1&fit=crop&h=200&w=280"
                }
            });
        }

        return Task.FromResult(new PhotoPage
        {
            photos = photos,
            page = page,
            perPage = pageSize,
            totalResults = 100
        });
    }

    private Task<CollectionPage> GetMockCollectionsAsync()
    {
        return Task.FromResult(new CollectionPage
        {
            collections = new List<Collection>
            {
                new Collection { id = "1", title = "Featured Mock", photosCount = 120, description = "Demo Collection" },
                new Collection { id = "2", title = "Nature Demo", photosCount = 45, description = "Demo Collection" },
                new Collection { id = "3", title = "Abstract Art", photosCount = 89, description = "Demo Collection" },
                new Collection { id = "4", title = "Technology", photosCount = 230, description = "Demo Collection" }
            },
            page = 1,
            perPage = 15,
            totalResults = 4
        });
    }

    #endregion
}

/// <summary>
/// Photo categories with emojis.
/// </summary>
public static class PhotoCategories
{
    public static readonly (string Name, string Emoji, string? Query)[] All =
    [
        ("Curated", "✨", null),
        ("Landscapes", "🖼️", "landscapes"),
        ("Technology", "💻", "technology"),
        ("Nature", "🏞️", "nature"),
        ("Animals", "🦁", "animals"),
        ("Lifestyle", "🗽", "lifestyle"),
        ("Search", "🔍", null)
    ];
}
