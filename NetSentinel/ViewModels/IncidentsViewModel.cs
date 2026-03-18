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

public partial class IncidentsViewModel : PaginatedViewModelBase
{
    private readonly IIncidentService _incidentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly IncidentDialogViewModel _incidentDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new();

    public IncidentsViewModel(IIncidentService incidentService, DialogManager dialogManager, ToastManager toastManager, IncidentDialogViewModel incidentDialogViewModel) : base()
    {
        Title = "Incidents";
        _incidentService = incidentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _incidentDialogViewModel = incidentDialogViewModel;
    }

    [RelayCommand]
    private Task LoadIncidentsAsync() => LoadPageAsync();

    protected override async Task LoadPageAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Incidents = new ObservableCollection<Incident>();

        try
        {
            var (items, totalCount) = await _incidentService.ReadIncidentsPagedAsync(CurrentPage, PageSize);
            Incidents = new ObservableCollection<Incident>(items);
            TotalCount = totalCount;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            IsEmpty = totalCount == 0;
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load incidents")
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
    private async Task DeleteIncidentAsync(Incident incident)
    {
        try
        {
            await _incidentService.DeleteIncidentAsync(incident);
            Incidents.Remove(incident);
            IsEmpty = Incidents.Count == 0;
            _toastManager
                .CreateToast("Incident deleted")
                .DismissOnClick()
                .ShowSuccess();
        }
        catch (DbUpdateException)
        {
            _toastManager
                .CreateToast("Cannot delete incident")
                .WithContent("This incident is still referenced by related data. Update or delete those records first.")
                .DismissOnClick()
                .ShowError();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to delete incident")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public async Task ShowNewIncidentDialog()
    {
        await _incidentDialogViewModel.LoadNewIncidentCommand.ExecuteAsync(null);
        _dialogManager.CreateDialog(_incidentDialogViewModel)
        .Dismissible()
        .WithSuccessCallback(() =>
        {
            _toastManager
                .CreateToast("Incident added")
                .DismissOnClick()
                .ShowSuccess();
            _ = LoadIncidentsAsync();

        })
        .Show();
    }

    [RelayCommand]
    public async Task ShowEditIncidentDialog(Incident incident)
    {
        await _incidentDialogViewModel.LoadExistingIncidentCommand.ExecuteAsync(incident);
        _dialogManager.CreateDialog(_incidentDialogViewModel)
        .Dismissible()
        .WithSuccessCallback(() =>
        {
            _toastManager
                .CreateToast("Incident edited")
                .DismissOnClick()
                .ShowSuccess();
            _ = LoadIncidentsAsync();
        })
        .Show();
    }

    [RelayCommand]
    public void ShowDeleteIncidentDialog(Incident incident)
    {
        _dialogManager.CreateDialog(
            "Are you certain?",
            "This action will permanently delete this incident.")
            .WithPrimaryButton("Confirm", async () =>
            {
                await _incidentService.DeleteIncidentAsync(incident);
                _toastManager.CreateToast("Incident deleted")
                .WithContent("Incident deleted successfully!")
                .DismissOnClick()
                .ShowSuccess();
                await LoadIncidentsAsync();
            }, DialogButtonStyle.Destructive)
            .WithCancelButton("Cancel")
            .WithMaxWidth(512)
            .Dismissible()
            .Show();
    }

}
