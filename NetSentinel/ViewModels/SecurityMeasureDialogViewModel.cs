using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    private SecurityMeasure _securityMeasure = new SecurityMeasure();
    private bool _isEditingExistingSecurityMeasure = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Description is required")]
    private string _description = String.Empty;

    [ObservableProperty]
    private string _result = String.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Performed by is required")]
    private Employee? _selectedEmployee;

    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident is required")]
    private Incident? _selectedIncident;

    [ObservableProperty]
    private bool _isLoading;

    public SecurityMeasureDialogViewModel(INavigationService navigator, DialogManager dialogManager, ISecurityMeasureService securityMeasureService, IEmployeeService employeeService, IIncidentService incidentService) : base(navigator)
    {
        Title = "Security Measure Dialog";
        _securityMeasureService = securityMeasureService;
        _employeeService = employeeService;
        _incidentService = incidentService;
        _dialogManager = dialogManager;
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

        try
        {
            if (_isEditingExistingSecurityMeasure)
                await _securityMeasureService.UpdateSecurityMeasureAsync(_securityMeasure);
            else
                await _securityMeasureService.AddSecurityMeasureAsync(_securityMeasure);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        catch
        {
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
        var employeeList = await _employeeService.ReadAllEmployeesAsync();
        Employees = new ObservableCollection<Employee>(employeeList);
        var incidentList = await _incidentService.ReadAllIncidentsAsync();
        Incidents = new ObservableCollection<Incident>(incidentList);
        IsLoading = false;

        _securityMeasure = securityMeasure;
        Description = securityMeasure.Description;
        Result = securityMeasure.Result;
        SelectedEmployee = securityMeasure.PerformedBy;
        SelectedIncident = securityMeasure.Incident;

        ClearErrors();
    }
}
