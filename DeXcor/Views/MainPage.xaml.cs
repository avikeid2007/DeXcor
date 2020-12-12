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
            var pexelsClient = new PexelsClient("563492ad6f91700001000001768850934594458b9990ea9808e8d8fb");
            PexelsDotNetSDK.Models.PhotoPage result = await pexelsClient.CuratedPhotosAsync();
        }
    }
}
