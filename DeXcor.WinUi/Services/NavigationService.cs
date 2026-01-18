using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls;

namespace DeXcor.WinUi.Services;

/// <summary>
/// Navigation service for WinUI NavigationView.
/// </summary>
public class NavigationService
{
    private Frame? _frame;
    private NavigationView? _navigationView;

    public Frame? Frame => _frame;

    public void Initialize(Frame frame, NavigationView navigationView)
    {
        _frame = frame;
        _navigationView = navigationView;
    }

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    public void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }

    public bool Navigate<T>(object? parameter = null) where T : Page
    {
        return Navigate(typeof(T), parameter);
    }

    public bool Navigate(Type pageType, object? parameter = null)
    {
        if (_frame is null) return false;

        // Don't navigate to the same page
        if (_frame.Content?.GetType() == pageType) return false;

        return _frame.Navigate(pageType, parameter);
    }

    public void SetSelectedNavigationItem(string tag)
    {
        if (_navigationView is null) return;

        foreach (var item in _navigationView.MenuItems.OfType<NavigationViewItem>())
        {
            if (item.Tag?.ToString() == tag)
            {
                _navigationView.SelectedItem = item;
                break;
            }
        }
    }
}
