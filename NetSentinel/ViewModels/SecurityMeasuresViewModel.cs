using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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

    private const int PageSize = 12;

    [ObservableProperty]
    private ObservableCollection<SecurityMeasure> _securityMeasures = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalCount;

    public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

    public SecurityMeasuresViewModel(INavigationService navigator, ISecurityMeasureService securityMeasureService, DialogManager dialogManager, ToastManager toastManager, SecurityMeasureDialogViewModel securityMeasureDialogViewModel) : base(navigator)
    {
        Title = "Security Measures";
        _securityMeasureService = securityMeasureService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _securityMeasureDialogViewModel = securityMeasureDialogViewModel;
    }

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageInfo));
    partial void OnTotalPagesChanged(int value) => OnPropertyChanged(nameof(PageInfo));

    public Task RefreshAsync() => LoadSecurityMeasuresAsync();

    [RelayCommand]
    private async Task LoadSecurityMeasuresAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        SecurityMeasures = new ObservableCollection<SecurityMeasure>();

        try
        {
            var (items, totalCount) = await _securityMeasureService.ReadSecurityMeasuresPagedAsync(CurrentPage, PageSize);
            SecurityMeasures = new ObservableCollection<SecurityMeasure>(items);
            TotalCount = totalCount;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            IsEmpty = totalCount == 0;
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

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadSecurityMeasuresAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadSecurityMeasuresAsync();
    }

    private bool CanGoPrevious() => CurrentPage > 1;
    private bool CanGoNext() => CurrentPage < TotalPages;

    [RelayCommand]
    public async Task ShowNewSecurityMeasureDialog()
    {
        try
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
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open new security measure")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public async Task ShowEditSecurityMeasureDialog(SecurityMeasure securityMeasure)
    {
        try
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
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open security measure")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public void ShowDeleteSecurityMeasureDialog(SecurityMeasure securityMeasure)
    {
        _dialogManager.CreateDialog(
            "Are you absolutely sure?",
            "This will permanently delete this security measure.")
        .WithPrimaryButton("Confirm", async () =>
        {
            try
            {
                await _securityMeasureService.DeleteSecurityMeasure(securityMeasure);
                _toastManager.CreateToast("Security measure deleted")
                    .WithContent("Security measure deleted successfully!")
                    .DismissOnClick()
                    .ShowSuccess();
                await LoadSecurityMeasuresAsync();
            }
            catch (DbUpdateException)
            {
                _toastManager
                    .CreateToast("Cannot delete security measure")
                    .WithContent("This security measure is still referenced by related data. Update or delete those records first.")
                    .DismissOnClick()
                    .ShowError();
            }
            catch (Exception ex)
            {
                _toastManager
                    .CreateToast("Failed to delete security measure")
                    .WithContent(ex.Message)
                    .DismissOnClick()
                    .ShowError();
            }
        }, DialogButtonStyle.Destructive)
        .WithCancelButton("Cancel")
        .WithMaxWidth(512)
        .Dismissible()
        .Show();
    }
}
