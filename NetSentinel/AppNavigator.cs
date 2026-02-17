using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.ViewModels;

namespace NetSentinel;

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

    public void NavigateTo(ViewModelBase viewModel)
    {
        _navigationStack.Push(CurrentView);
        CurrentView = viewModel;
    }

    public void NavigateTo<T>() where T : ViewModelBase, new()
    {
        NavigateTo(new T());
    }

    public bool GoBack()
    {
        if (_navigationStack.Count == 0)
            return false;

        CurrentView = _navigationStack.Pop();
        return true;
    }

    public bool CanGoBack => _navigationStack.Count > 0;

    [RelayCommand]
    private void GoToTest1() => NavigateTo<Test1ViewModel>();

    [RelayCommand]
    private void GoToTest2() => NavigateTo<Test2ViewModel>();

    [RelayCommand]
    private void NavigateBack() => GoBack();
}
