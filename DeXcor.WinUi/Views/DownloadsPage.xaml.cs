using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DeXcor.WinUi.Views;

/// <summary>
/// Page for managing downloads.
/// </summary>
public sealed partial class DownloadsPage : Page
{
    public bool HasActiveDownloads => App.Downloads.ActiveDownloads.Count > 0;

    public DownloadsPage()
    {
        this.InitializeComponent();

        ActiveDownloadsList.ItemsSource = App.Downloads.ActiveDownloads;
        CompletedDownloadsList.ItemsSource = App.Downloads.CompletedDownloads;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        UpdateUI();
    }

    private void UpdateUI()
    {
        TotalCountText.Text = App.Downloads.TotalDownloaded.ToString();
        EmptyState.Visibility = App.Downloads.CompletedDownloads.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private async void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        await App.Downloads.OpenDownloadsFolderAsync();
    }
}
