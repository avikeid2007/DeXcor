using DeXcor.Views.ViewModels;
using PexelsDotNetSDK.Api;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DeXcor.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; set; }
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

        }

        private static async Task NewMethod()
        {
            var pexelsClient = new PexelsClient("");
            PexelsDotNetSDK.Models.PhotoPage result = await pexelsClient.CuratedPhotosAsync();
        }
    }
}
