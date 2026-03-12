using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

/// <summary>
/// Viewmodel for the main window. Implements logic
/// for the sidebar with buttons for switching view
/// and to toggle theme (Dark / Light mode).
/// </summary>

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ThemeToggleText))]
    [NotifyPropertyChangedFor(nameof(ThemeToggleIcon))]
    private bool _isDarkMode;
    public string ThemeToggleText => IsDarkMode ? "Light Mode" : "Dark Mode";
    public string ThemeToggleIcon => IsDarkMode ? "Sun" : "Moon";

    [ObservableProperty]
    private DialogManager _dialogManager;

    [ObservableProperty]
    private ToastManager _toastManager;

    public MainWindowViewModel(NavigationService navigator, DialogManager dialogManager, ToastManager toastManager) : base(navigator)
    {
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        if (Application.Current != null)
        {
            // ActualThemeVariant resolves "Default" into either Light or Dark
            IsDarkMode = Application.Current.ActualThemeVariant == ThemeVariant.Dark;
        }
        Navigator.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Navigator.CurrentView))
                OnPropertyChanged(nameof(CanRefresh));
        };
    }

    public bool CanRefresh => Navigator.CurrentView is IRefreshable;

    [RelayCommand]
    private async Task RefreshCurrentViewAsync()
    {
        if (Navigator.CurrentView is IRefreshable refreshable)
            await refreshable.RefreshAsync();
    }

    [RelayCommand]
    public void GoToDashboard()
    {
        Navigator.GoToDashboard();
    }

    [RelayCommand]
    public void GoToDevices()
    {
        Navigator.GoToDevices();
    }

    [RelayCommand]
    public void GoToDepartments()
    {
        Navigator.GoToDepartments();
    }

    [RelayCommand]
    public void GoToIncidents()
    {
        Navigator.GoToIncidents();
    }

    [RelayCommand]
    public void GoToEmployees()
    {
        Navigator.GoToEmployees();
    }

    [RelayCommand]
    public void GoToSecurityMeasures()
    {
        Navigator.GoToSecurityMeasures();
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
