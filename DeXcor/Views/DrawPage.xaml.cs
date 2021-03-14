using DeXcor.Behaviors;
using DeXcor.Helpers;
using DeXcor.Services.Ink;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace DeXcor.Views
{
    // For more information regarding Windows Ink documentation and samples see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/ink.md
    public sealed partial class DrawPage : Page, INotifyPropertyChanged
    {
        private StorageFile imageFile;

        private bool touchInkingButtonIsChecked = true;
        private bool mouseInkingButtonIsChecked = true;
        private bool saveImageButtonIsEnabled;
        private bool clearAllButtonIsEnabled;
        private InkStrokesService strokesService;
        private InkPointerDeviceService pointerDeviceService;
        private InkFileService fileService;
        private InkZoomService zoomService;
        private bool isImageEdit;

        public DrawPage()
        {
            InitializeComponent();
            SetNavigationViewHeaderContext();
            SetNavigationViewHeaderTemplate();

            Loaded += (_, _) =>
            {
                SetCanvasSize();
                image.SizeChanged += Image_SizeChanged;
                strokesService = new InkStrokesService(inkCanvas.InkPresenter);
                pointerDeviceService = new InkPointerDeviceService(inkCanvas);
                fileService = new InkFileService(inkCanvas, strokesService);
                zoomService = new InkZoomService(canvasScroll);
                strokesService.StrokesCollected += (_, _) => RefreshEnabledButtons();
                strokesService.StrokesErased += (_, _) => RefreshEnabledButtons();
                strokesService.ClearStrokesEvent += (_, _) => RefreshEnabledButtons();
                pointerDeviceService.DetectPenEvent += (_, _) => TouchInkingButtonIsChecked = false;
                _ = LoadPlainAsync();
            };
        }

        private void OnInkToolbarLoaded(object sender, RoutedEventArgs _)
        {
            if (sender is InkToolbar inkToolbar)
            {
                inkToolbar.TargetInkCanvas = inkCanvas;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is StorageFile file)
            {
                isImageEdit = true;
                _ = LoadImageAsync(file);
            }
            base.OnNavigatedTo(e);
        }
        private void VisualStateGroup_CurrentStateChanged(object _, VisualStateChangedEventArgs __) => SetNavigationViewHeaderTemplate();

        private void SetNavigationViewHeaderTemplate()
        {
            if (visualStateGroup.CurrentState != null)
            {
                switch (visualStateGroup.CurrentState.Name)
                {
                    case "BigVisualState":
                        NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["BigHeaderTemplate"] as DataTemplate);
                        bottomCommandBar.Visibility = Visibility.Collapsed;
                        break;
                    case "SmallVisualState":
                        NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["SmallHeaderTemplate"] as DataTemplate);
                        bottomCommandBar.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void SetNavigationViewHeaderContext()
        {
            var headerContextBinding = new Binding
            {
                Source = this,
                Mode = BindingMode.OneWay,
            };

            SetBinding(NavigationViewHeaderBehavior.HeaderContextProperty, headerContextBinding);
        }

        public bool TouchInkingButtonIsChecked
        {
            get => touchInkingButtonIsChecked;
            set
            {
                Set(ref touchInkingButtonIsChecked, value);
                pointerDeviceService.EnableTouch = value;
            }
        }

        public bool MouseInkingButtonIsChecked
        {
            get => mouseInkingButtonIsChecked;
            set
            {
                Set(ref mouseInkingButtonIsChecked, value);
                pointerDeviceService.EnableMouse = value;
            }
        }

        public bool SaveImageButtonIsEnabled
        {
            get => saveImageButtonIsEnabled;
            set => Set(ref saveImageButtonIsEnabled, value);
        }

        public bool ClearAllButtonIsEnabled
        {
            get => clearAllButtonIsEnabled;
            set => Set(ref clearAllButtonIsEnabled, value);
        }

        private void SetCanvasSize()
        {
            inkCanvas.Width = Math.Max(canvasScroll.ViewportWidth, 1000);
            inkCanvas.Height = Math.Max(canvasScroll.ViewportHeight, 1000);
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height == 0 || e.NewSize.Width == 0)
            {
                SetCanvasSize();
            }
            else
            {
                inkCanvas.Width = e.NewSize.Width;
                inkCanvas.Height = e.NewSize.Height;
            }
        }

        private void ZoomIn_Click(object _, RoutedEventArgs __) => zoomService?.ZoomIn();

        private void ZoomOut_Click(object _, RoutedEventArgs __) => zoomService?.ZoomOut();

        private void ResetZoom_Click(object _, RoutedEventArgs __) => zoomService?.ResetZoom();

        private void FitToScreen_Click(object _, RoutedEventArgs __) => zoomService?.FitToScreen();

        private async void LoadImage_Click(object _, RoutedEventArgs __)
        {
            var file = await ImageHelper.LoadImageFileAsync();
            await LoadImageAsync(file);
        }
        private async Task LoadImageAsync(StorageFile file)
        {
            var bitmapImage = await ImageHelper.GetBitmapFromImageAsync(file);
            if (file != null && bitmapImage != null)
            {
                ClearAll();
                imageFile = file;
                image.Source = bitmapImage;
                zoomService?.FitToSize(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                RefreshEnabledButtons();
            }
        }
        private async Task LoadPlainAsync()
        {
            if (!isImageEdit)
            {
                var InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var file = await InstallationFolder.GetFileAsync(@"Assets\SampleData\plain.jpg");
                if (file != null)
                {
                    await LoadImageAsync(file);
                }
            }
        }
        private async void SaveImage_Click(object _, RoutedEventArgs __) => await fileService?.ExportToImageAsync(imageFile);

        private void ClearAll_Click(object _, RoutedEventArgs __) => ClearAll();

        private void ClearAll()
        {
            strokesService?.ClearStrokes();
            imageFile = null;
            image.Source = null;
            RefreshEnabledButtons();
        }

        private void RefreshEnabledButtons()
        {
            SaveImageButtonIsEnabled = image.Source != null && strokesService.GetStrokes().Any();
            ClearAllButtonIsEnabled = image.Source != null || strokesService.GetStrokes().Any();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void SetImage_Click(object _, RoutedEventArgs __)
        {
            bool IsChanged;
            if (isImageEdit)
            {
                IsChanged = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(imageFile);
            }
            else
            {
                var file = await fileService?.ExportFileWithImage(imageFile, await ImageHelper.CreateImageFile());
                IsChanged = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
            }
            if (IsChanged)
            {
                await DialogHelper.ShowDialogAsync("This image has set as you PC background😍😍.", "PC Background Changed❤❤");
            }
        }
    }
}
