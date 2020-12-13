using BasicMvvm;
using BasicMvvm.Commands;
using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.Web.Http;

namespace DeXcor.Views.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private PhotoPage _currentPage;
        private List<Photo> _usedList;
        private Photo _selectedPhoto;
        public Photo SelectedPhoto
        {
            get { return _selectedPhoto; }
            set
            {
                _selectedPhoto = value;
                OnPropertyChanged();
            }
        }
        public ICommand ChangeBackgroundCommand => new AsyncCommand(OnChangeBackgroundExecutedAsync);

        private async Task OnChangeBackgroundExecutedAsync()
        {
            await ChangeBackgroundAsync();
        }

        private async Task ChangeBackgroundAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                try
                {
                    if (_currentPage == null)
                    {
                        _currentPage = await FetchWallPaperListAsync();
                    }
                    if (_usedList == null)
                    {
                        _usedList = new List<Photo>();
                    }
                    using (HttpClient client = new HttpClient())
                    {
                        SelectedPhoto = await GetNewPhotoAsync();
                        HttpResponseMessage response = await client.GetAsync(new Uri(SelectedPhoto.source.landscape));
                        if (response != null && response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                        {
                            string filename = "background.jpg";
                            var imageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                            using (IRandomAccessStream stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                ulong ss = await response.Content.WriteToStreamAsync(stream);
                            }
                            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                            UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                            if (!await settings.TrySetWallpaperImageAsync(file))
                            {
                                System.Diagnostics.Debug.WriteLine("Failed");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Success");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private async Task<Photo> GetNewPhotoAsync()
        {
            if (_currentPage != null)
            {
                var photo = _currentPage?.photos?.FirstOrDefault(x => !_usedList.Contains(x));
                if (photo == null)
                {
                    _currentPage = await FetchWallPaperListAsync(page: _currentPage.page + 1);
                    photo = _currentPage?.photos?.FirstOrDefault(x => _usedList.Contains(x));
                    if (photo != null)
                    {
                        return photo;
                    }
                }
                else
                {
                    return photo;
                }
            }
            return null;
        }

        private async Task<PhotoPage> FetchWallPaperListAsync(int page = 1)
        {
            //string apiKey = ConfigurationManager.AppSettings["apiKey"];
            if (!string.IsNullOrEmpty(App.ApiKey))
            {
                var pexelsClient = new PexelsClient(App.ApiKey);
                return await pexelsClient.CuratedPhotosAsync(page: page, pageSize: 80);
            }
            return null;
        }
    }
}
