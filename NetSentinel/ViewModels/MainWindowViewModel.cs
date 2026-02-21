using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ThemeToggleText))]
    [NotifyPropertyChangedFor(nameof(ThemeToggleIcon))]
    private bool _isDarkMode;
    public string ThemeToggleText => IsDarkMode ? "Light Mode" : "Dark Mode";
    public string ThemeToggleIcon => IsDarkMode ? "Sun" : "Moon";

    public MainWindowViewModel(IAppNavigator navigator) : base(navigator)
    {
        if (Application.Current != null)
        {
            // ActualThemeVariant resolves "Default" into either Light or Dark
            IsDarkMode = Application.Current.ActualThemeVariant == ThemeVariant.Dark;
        }
    }

    [RelayCommand]
    public void GoToDashboard()
    {
        Navigator.GoToDashboard();
    }

    [RelayCommand]
    public void GoToEmployees()
    {
        Navigator.GoToEmployees();
    }

    [RelayCommand]
    public void GoToSettings()
    {
        Navigator.GoToSettings();
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;

        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = IsDarkMode
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }
    }
}
