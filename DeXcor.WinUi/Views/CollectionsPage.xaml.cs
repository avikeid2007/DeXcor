using DeXcor.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DeXcor.WinUi.Views;

/// <summary>
/// Page for browsing Pexels featured collections.
/// </summary>
public sealed partial class CollectionsPage : Microsoft.UI.Xaml.Controls.Page
{
    public CollectionsViewModel ViewModel { get; } = new();

    public CollectionsPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Only load if not already loaded
        if (ViewModel.Collections.Count == 0)
        {
            LoadingRing.IsActive = true;
            LoadingRing.Visibility = Visibility.Visible;
            CollectionsGrid.Visibility = Visibility.Collapsed;

            await ViewModel.LoadCollectionsCommand.ExecuteAsync(null);

            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
            CollectionsGrid.Visibility = Visibility.Visible;
            LoadMoreButton.Visibility = Visibility.Visible;
        }

        // Bind the grid
        CollectionsGrid.ItemsSource = ViewModel.Collections;
    }

    private void Collection_Click(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is CollectionItemViewModel vm)
        {
            // TODO: Navigate to collection detail page
            // Frame.Navigate(typeof(CollectionDetailPage), vm.Model);
        }
    }

    private void CollectionButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.DataContext is CollectionItemViewModel vm)
        {
            // Frame.Navigate(typeof(CollectionDetailPage), vm.Model);
        }
    }

    private async void LoadMore_Click(object sender, RoutedEventArgs e)
    {
        LoadMoreButton.IsEnabled = false;
        await ViewModel.LoadCollectionsCommand.ExecuteAsync(null);
        LoadMoreButton.IsEnabled = true;
    }
}

