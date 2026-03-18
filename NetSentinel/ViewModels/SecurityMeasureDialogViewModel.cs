using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class SecurityMeasureDialogViewModel : ViewModelBase
{
    private readonly ISecurityMeasureService _securityMeasureService;
    private readonly IEmployeeService _employeeService;
    private readonly IIncidentService _incidentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    private SecurityMeasure _securityMeasure = new SecurityMeasure();
    private bool _isEditingExistingSecurityMeasure = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(400, ErrorMessage = "Description cannot exceed 400 characters")]
    private string _description = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Result is required")]
    [MaxLength(200, ErrorMessage = "Result cannot exceed 200 characters")]
    private string _result = String.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Performed by is required")]
    private Employee? _selectedEmployee;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Performed at is required")]
    private DateTime _performedAt = DateTime.Now;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Performed date is required")]
    private DateTime? _performedDate;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Performed time is required")]
    private TimeSpan? _performedTime;

    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident is required")]
    private Incident? _selectedIncident;

    [ObservableProperty]
    private bool _isLoading;

    public SecurityMeasureDialogViewModel(DialogManager dialogManager, ToastManager toastManager, ISecurityMeasureService securityMeasureService, IEmployeeService employeeService, IIncidentService incidentService) : base()
    {
        Title = "Security Measure Dialog";
        _securityMeasureService = securityMeasureService;
        _employeeService = employeeService;
        _incidentService = incidentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task LoadNewSecurityMeasureAsync()
    {
        Title = "Add Security Measure";
        _isEditingExistingSecurityMeasure = false;
        await InitializeProperties(new SecurityMeasure());
    }

    [RelayCommand]
    public async Task LoadExistingSecurityMeasureAsync(SecurityMeasure securityMeasure)
    {
        Title = "Edit Security Measure";
        _isEditingExistingSecurityMeasure = true;
        await InitializeProperties(securityMeasure);
    }

    [RelayCommand]
    private async Task SaveSecurityMeasureAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
            return;

        _securityMeasure.Description = Description;
        _securityMeasure.Result = Result;
        _securityMeasure.PerformedBy = SelectedEmployee;
        _securityMeasure.Incident = SelectedIncident;
        _securityMeasure.PerformedAt = PerformedAt;

        try
        {
            if (_isEditingExistingSecurityMeasure)
                await _securityMeasureService.UpdateSecurityMeasureAsync(_securityMeasure);
            else
                await _securityMeasureService.AddSecurityMeasureAsync(_securityMeasure);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        catch (InvalidOperationException ex)
        {
            _toastManager
                .CreateToast("Failed to save security measure")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        catch
        {
            _toastManager
                .CreateToast("Failed to save security measure")
                .DismissOnClick()
                .ShowError();
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        finally
        {
            _dialogManager.Dispose();
        }
    }

    private async Task InitializeProperties(SecurityMeasure securityMeasure)
    {
        IsLoading = true;
        try
        {
            var employeeList = await _employeeService.ReadAllEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employeeList);
            var incidentList = await _incidentService.ReadAllIncidentsAsync();
            Incidents = new ObservableCollection<Incident>(incidentList);
        }
        catch (Exception ex)
        {
            Employees = new ObservableCollection<Employee>();
            Incidents = new ObservableCollection<Incident>();
            _toastManager
                .CreateToast("Failed to load employees or incidents")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
        finally
        {
            IsLoading = false;
        }

        _securityMeasure = securityMeasure;
        Description = securityMeasure.Description;
        Result = securityMeasure.Result;
        SelectedEmployee = securityMeasure.PerformedBy is null
            ? null
            : Employees.FirstOrDefault(e => e.Id == securityMeasure.PerformedBy.Id) ?? securityMeasure.PerformedBy;
        SelectedIncident = securityMeasure.Incident is null
            ? null
            : Incidents.FirstOrDefault(i => i.Id == securityMeasure.Incident.Id) ?? securityMeasure.Incident;
        PerformedAt = securityMeasure.PerformedAt == default
            ? DateTime.Now
            : securityMeasure.PerformedAt;
        PerformedDate = PerformedAt.Date;
        PerformedTime = PerformedAt.TimeOfDay;

        ClearErrors();
    }

    private void UpdatePerformedAtFromPickers()
    {
        if (PerformedDate.HasValue && PerformedTime.HasValue)
        {
            PerformedAt = PerformedDate.Value.Date + PerformedTime.Value;
        }
    }

    partial void OnPerformedDateChanged(DateTime? value)
    {
        UpdatePerformedAtFromPickers();
    }

    partial void OnPerformedTimeChanged(TimeSpan? value)
    {
        UpdatePerformedAtFromPickers();
    }
}
