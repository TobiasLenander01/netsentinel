using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
    private readonly IDeviceService _deviceService;
    private readonly ToastManager _toastManager;
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
    [Required(ErrorMessage = "Incident date is required")]
    private DateTime? _createdAtDate;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident time is required")]
    private TimeSpan? _createdAtTime = DateTime.Now.TimeOfDay;

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

    [ObservableProperty]
    private ObservableCollection<Device> _devices = new();

    public IncidentDialogViewModel(
        DialogManager dialogManager,
        ToastManager toastManager,
        IIncidentService incidentService,
        IDeviceService deviceService) : base()
    {
        Title = "Incident Dialog";
        _incidentService = incidentService;
        _deviceService = deviceService;
        _toastManager = toastManager;
        _dialogManager = dialogManager;
    }

    [RelayCommand]
    private async Task LoadNewIncidentAsync()
    {
        Title = "Add Incident";
        _isEditingExistingIncident = false;
        await InitializePropertiesAsync(new Incident { CreatedAt = DateTime.Now });

    }

    [RelayCommand]
    public async Task LoadExistingIncidentAsync(Incident incident)
    {
        Title = "Edit Incident";
        _isEditingExistingIncident = true;
        await InitializePropertiesAsync(incident);

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

    private async Task InitializePropertiesAsync(Incident incident)
    {
        try
        {
            var list = await _deviceService.ReadAllDevicesAsync();
            Devices = new ObservableCollection<Device>(list);
        }
        catch (Exception ex)
        {
            Devices = new ObservableCollection<Device>();
            _toastManager
                .CreateToast("Failed to load devices")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }

        _incident = incident;
        var createdAtValue = incident.CreatedAt == default ? DateTime.Now : incident.CreatedAt;
        Description = incident.Description;
        CreatedAt = createdAtValue;
        CreatedAtDate = createdAtValue.Date;
        CreatedAtTime = createdAtValue.TimeOfDay;
        Priority = incident.Priority;
        Status = incident.Status;
        Device = incident.Device;

        ClearErrors();
    }

    public Array Priorities => Enum.GetValues<Priority>();
    public Array Statuses => Enum.GetValues<Status>();

    private void UpdateCreatedAtFromPickers()
    {
        if (CreatedAtDate.HasValue && CreatedAtTime.HasValue)
        {
            var localDateTime = CreatedAtDate.Value.Date + CreatedAtTime.Value;
            CreatedAt = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
        }
    }

    partial void OnCreatedAtDateChanged(DateTime? value)
    {
        UpdateCreatedAtFromPickers();
    }

    partial void OnCreatedAtTimeChanged(TimeSpan? value)
    {
        UpdateCreatedAtFromPickers();
    }
}
