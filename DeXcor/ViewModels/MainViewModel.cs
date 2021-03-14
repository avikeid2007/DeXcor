using BasicMvvm;
using BasicMvvm.Commands;
using DeXcor.Helpers;
using DeXcor.Services;
using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System.UserProfile;

namespace DeXcor.ViewModels
{
    public class MainViewModel : BindableBase
    {

        private Photo _selectedPhoto;
        private bool _isBusy;
        private bool _isNewBackground;
        public bool IsNewBackground
        {
            get { return _isNewBackground; }
            set
            {
                _isNewBackground = value;
                OnPropertyChanged();
            }
        }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                IsNewBackground = !value;
                OnPropertyChanged();
            }
        }
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
            if (ImageDataService.UsedList == null)
            {
                ImageDataService.UsedList = new List<Photo>();
            }
            if (UserProfilePersonalizationSettings.IsSupported() && ImageDataService.CuratedWallpaperCollection?.Count > 0)
            {
                try
                {
                    IsBusy = true;
                    SelectedPhoto = await GetNewPhotoAsync();
                    if (SelectedPhoto != null)
                    {
                        await BackgroundHelper.SetBackgroundAsync(SelectedPhoto);
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.LogException(ex);
                    await DialogHelper.ShowDialogAsync("Something went wrong.");
                }
                finally
                {
                    IsBusy = false;
                }
            }

        }



        private async Task<Photo> GetNewPhotoAsync()
        {
            if (ImageDataService.CuratedWallpaperCollection?.Count > 0)
            {
                var photo = ImageDataService.CuratedWallpaperCollection.FirstOrDefault(x => !ImageDataService.UsedList.Contains(x));
                if (photo == null)
                {
                    await ImageDataService.FetchHomeWallPaperListAsync(page: ImageDataService.photoPage.page + 1);
                    return await GetNewPhotoAsync();
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
            if (!string.IsNullOrEmpty(ImageDataService.ApiKey))
            {
                var pexelsClient = new PexelsClient(ImageDataService.ApiKey);
                return await pexelsClient.CuratedPhotosAsync(page: page, pageSize: 80);
            }
            return null;
        }
    }
}
