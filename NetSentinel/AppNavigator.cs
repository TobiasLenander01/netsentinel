using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.ViewModels;

namespace NetSentinel;

/// <summary>
/// The purpose of this class is to manage the navigation between different views in the application.
/// It maintains a stack of view models to allow for navigating back to previous views. 
/// The AppNavigator has a public static instance that is available to all view models which
/// removes the need for dependency injection and allows for easy navigation from any view model.
/// </summary>

public partial class AppNavigator : ObservableObject
{
    public static AppNavigator Instance { get; } = new();
    private readonly Stack<ViewModelBase> _navigationStack = new();

    [ObservableProperty]
    private ViewModelBase _currentView;

    private AppNavigator()
    {
        _currentView = new Test1ViewModel();
    }

    private void NavigateTo(ViewModelBase viewModel)
    {
        _navigationStack.Push(CurrentView);
        CurrentView = viewModel;
    }

    [RelayCommand]
    public void GoBack()
    {
        if (_navigationStack.Count == 0)
            return;

        CurrentView = _navigationStack.Pop();
    }

    [RelayCommand]
    public void GoToTest1()
    {
        NavigateTo(new Test1ViewModel());
    }

    [RelayCommand]
    public void GoToTest2()
    {
        NavigateTo(new Test2ViewModel());
    }

    public bool CanGoBack()
    {
        return _navigationStack.Count > 0;
    }
}
