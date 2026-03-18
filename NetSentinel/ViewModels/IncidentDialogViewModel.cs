using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Enums;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class IncidentDialogViewModel : ViewModelBase 
{
    private readonly IIncidentService _incidentService;
    private readonly DialogManager _dialogManager;

    private Incident _incident = new Incident();

    private bool _isEditingExistingIncident = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Description is required")]
    private string _description = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Date and time of incident is required")]
    private DateTime _createdAt = DateTime.Now;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Choose a priority")]
    private Priority? _priority = null;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Choose status")]
    private Status? _status = null;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Choose an option for device")]
    private Device? _device = null;

    public IncidentDialogViewModel(DialogManager dialogManager, IIncidentService incidentService) : base()
    {
        Title = "Incident Dialog";
        _incidentService = incidentService;
        _dialogManager = dialogManager;
    }

    [RelayCommand]
    private Task LoadNewIncidentAsync()
    {
        Title = "Add Incident";
        _isEditingExistingIncident = false;
        InitializeProperties(new Incident());
        return Task.CompletedTask;

    }

    [RelayCommand]
    public Task LoadExistingIncidentAsync(Incident incident)
    {
        Title = "Edit Incident";
        _isEditingExistingIncident = true;
        InitializeProperties(incident);
        return Task.CompletedTask;

    }

    [RelayCommand]
    private async Task SaveIncidentAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
            return;

        _incident.Description = Description;
        _incident.CreatedAt = CreatedAt;
        _incident.Priority = Priority!.Value;
        _incident.Status = Status!.Value;
        _incident.Device = Device;

        try
        {
            if(_isEditingExistingIncident)
                await _incidentService.UpdateIncidentAsync(_incident);
            else
                await _incidentService.AddIncidentAsync(_incident);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true});
        }
        catch
        {
            _dialogManager.Close(this, new CloseDialogOptions { Success = false});
        }
        finally
        {
            _dialogManager.Dispose();
        }
    }

    private void InitializeProperties(Incident incident)
    {
        _incident = incident;
        Description = incident.Description;
        CreatedAt = incident.CreatedAt;
        Priority = incident.Priority;
        Status = incident.Status;
        Device = incident.Device;

        ClearErrors();
    }

}
