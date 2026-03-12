using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class SecurityMeasuresViewModel : ViewModelBase, IRefreshable
{
    private readonly ISecurityMeasureService _securityMeasureService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly SecurityMeasureDialogViewModel _securityMeasureDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<SecurityMeasure> _securityMeasures = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public SecurityMeasuresViewModel(INavigationService navigator, ISecurityMeasureService securityMeasureService, DialogManager dialogManager, ToastManager toastManager, SecurityMeasureDialogViewModel securityMeasureDialogViewModel) : base(navigator)
    {
        Title = "Security Measures";
        _securityMeasureService = securityMeasureService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _securityMeasureDialogViewModel = securityMeasureDialogViewModel;
    }

    public Task RefreshAsync() => LoadSecurityMeasuresAsync();

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

    [RelayCommand]
    public async Task ShowNewSecurityMeasureDialog()
    {
        await _securityMeasureDialogViewModel.LoadNewSecurityMeasureCommand.ExecuteAsync(null);
        _dialogManager.CreateDialog(_securityMeasureDialogViewModel)
        .Dismissible()
        .WithSuccessCallback(() =>
            {
                _toastManager
                    .CreateToast("Security measure added")
                    .DismissOnClick()
                    .ShowSuccess();
                _ = LoadSecurityMeasuresAsync();
            })
        .Show();
    }

    [RelayCommand]
    public async Task ShowEditSecurityMeasureDialog(SecurityMeasure securityMeasure)
    {
        await _securityMeasureDialogViewModel.LoadExistingSecurityMeasureCommand.ExecuteAsync(securityMeasure);
        _dialogManager.CreateDialog(_securityMeasureDialogViewModel)
        .Dismissible()
        .WithSuccessCallback(() =>
            {
                _toastManager
                    .CreateToast("Security measure updated")
                    .DismissOnClick()
                    .ShowSuccess();
                _ = LoadSecurityMeasuresAsync();
            })
        .Show();
    }

    [RelayCommand]
    public void ShowDeleteSecurityMeasureDialog(SecurityMeasure securityMeasure)
    {
        _dialogManager.CreateDialog(
            "Are you absolutely sure?",
            "This will permanently delete this security measure.")
        .WithPrimaryButton("Confirm", async () =>
        {
            await _securityMeasureService.DeleteSecurityMeasure(securityMeasure);
            _toastManager.CreateToast("Security measure deleted")
                .WithContent("Security measure deleted successfully!")
                .DismissOnClick()
                .ShowSuccess();
            await LoadSecurityMeasuresAsync();
        }, DialogButtonStyle.Destructive)
        .WithCancelButton("Cancel")
        .WithMaxWidth(512)
        .Dismissible()
        .Show();
    }
}
