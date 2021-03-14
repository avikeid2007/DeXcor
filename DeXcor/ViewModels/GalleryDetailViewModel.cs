using BasicMvvm;
using BasicMvvm.Commands;
using DeXcor.Helpers;
using DeXcor.Services;
using DeXcor.Views;
using PexelsDotNetSDK.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeXcor.ViewModels
{
    public class GalleryDetailViewModel : BindableBase
    {
        public ICommand DownloadCommand => new AsyncCommand<string>(OnDownloadCommandExecutedAsync);
        public ICommand SetBackgroundCommand => new AsyncCommand(OnSetBackgroundCommandExecutedAsync);
        public ICommand EditCommand => new AsyncCommand(OnEditCommandExecutedAsync);

        private async Task OnEditCommandExecutedAsync()
        {
            var sampleFile = await ImageHelper.CreateImageFile();
            if (SelectedImage != null && await ImageHelper.DownloadFileFromURLAsync(sampleFile, GetImageUrl("large2x")))
                NavigationService.Navigate(typeof(DrawPage), sampleFile);
        }


        private Photo _selectedImage;
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        public Photo SelectedImage
        {
            get => _selectedImage;
            set
            {
                Set(ref _selectedImage, value);
                ImagesNavigationHelper.UpdateImageId(ImageDataService.GallerySelectedIdKey, SelectedImage.id);
            }
        }
        public ObservableCollection<Photo> Source { get; } = new ObservableCollection<Photo>(ImageDataService.ImageCollection);
        private async Task OnDownloadCommandExecutedAsync(string size)
        {
            try
            {
                if (SelectedImage != null)
                {
                    IsBusy = true;
                    var url = GetImageUrl(size.ToLower());
                    if (!string.IsNullOrEmpty(url) && await ImageHelper.DownloadImageAsync(url, $"{SelectedImage.id}.jpg"))
                    {
                        await DialogHelper.ShowDialogAsync("Download complete.");
                        IsBusy = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetry.LogException(ex);
                IsBusy = false;
                await DialogHelper.ShowDialogAsync("Something went wrong.");
            }
        }

        private string GetImageUrl(string size)
        {
            return size switch
            {
                "original" => SelectedImage.source.original,
                "large2x" => SelectedImage.source.large2x,
                "large" => SelectedImage.source.large,
                "small" => SelectedImage.source.small,
                "tiny" => SelectedImage.source.tiny,
                _ => string.Empty
            };
        }

        private async Task OnSetBackgroundCommandExecutedAsync()
        {
            try
            {
                IsBusy = true;
                await BackgroundHelper.SetBackgroundAsync(SelectedImage, false);
                await DialogHelper.ShowDialogAsync("This image has set as you PC background😍😍.", "PC Background Changed❤❤");
                IsBusy = false;

            }
            catch (Exception ex)
            {
                Telemetry.LogException(ex);
                IsBusy = false;
                await DialogHelper.ShowDialogAsync("Something went wrong.");
            }
        }
    }
}
