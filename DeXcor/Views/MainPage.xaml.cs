using DeXcor.ViewModels;
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
    }
}
