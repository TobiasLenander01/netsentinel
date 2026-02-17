using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NetSentinel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel()
    {
        _currentView = new Test1ViewModel();
    }

    [RelayCommand]
    private void GoToTest1()
    {
        CurrentView = new Test1ViewModel();
    }

    [RelayCommand]
    private void GoToTest2()
    {
        CurrentView = new Test2ViewModel();
    }
}
