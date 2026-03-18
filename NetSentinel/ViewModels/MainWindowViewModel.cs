using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using ShadUI;

namespace NetSentinel.ViewModels;

/// <summary>
/// Viewmodel for the main window. Implements logic
/// for the sidebar with buttons for switching view
/// and to toggle theme (Dark / Light mode).
/// </summary>

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigator;
    public INavigationService Navigator => _navigator;

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

    public MainWindowViewModel(INavigationService navigator, DialogManager dialogManager, ToastManager toastManager) : base()
    {
        _navigator = navigator;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        if (Application.Current != null)
        {
            // ActualThemeVariant resolves "Default" into either Light or Dark
            IsDarkMode = Application.Current.ActualThemeVariant == ThemeVariant.Dark;
        }
        _navigator.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_navigator.CurrentView))
                OnPropertyChanged(nameof(CanRefresh));
        };
    }

    public bool CanRefresh => _navigator.CurrentView is IRefreshable;

    [RelayCommand]
    private async Task RefreshCurrentViewAsync()
    {
        if (_navigator.CurrentView is IRefreshable refreshable)
            await refreshable.RefreshAsync();
    }

    [RelayCommand]
    public void GoToDashboard()
    {
        _navigator.GoToDashboard();
    }

    [RelayCommand]
    public void GoToDevices()
    {
        _navigator.GoToDevices();
    }

    [RelayCommand]
    public void GoToDepartments()
    {
        _navigator.GoToDepartments();
    }

    [RelayCommand]
    public void GoToIncidents()
    {
        _navigator.GoToIncidents();
    }

    [RelayCommand]
    public void GoToEmployees()
    {
        _navigator.GoToEmployees();
    }

    [RelayCommand]
    public void GoToSecurityMeasures()
    {
        _navigator.GoToSecurityMeasures();
    }

    [RelayCommand]
    public void GoToSettings()
    {
        _navigator.GoToSettings();
    }

    [RelayCommand]
    public void GoToAIReport()
    {
        _navigator.GoToAIReport();
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
