using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class SecurityMeasuresViewModel : ViewModelBase
{
    private readonly SecurityMeasureService _securityMeasureService;
    private readonly ToastManager _toastManager;

    [ObservableProperty]
    private ObservableCollection<SecurityMeasure> _securityMeasures = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public SecurityMeasuresViewModel(NavigationService navigator, SecurityMeasureService securityMeasureService, ToastManager toastManager) : base(navigator)
    {
        Title = "Security Measures";
        _securityMeasureService = securityMeasureService;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task LoadSecurityMeasuresAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        SecurityMeasures = new ObservableCollection<SecurityMeasure>();

        try
        {
            var securityMeasureList = await _securityMeasureService.ReadAllSecurityMeasuresAsync();
            SecurityMeasures = new ObservableCollection<SecurityMeasure>(securityMeasureList);
            IsEmpty = SecurityMeasures.Count == 0;
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load security measures")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
