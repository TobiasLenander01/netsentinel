using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class EmployeeDialogViewModel : ViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    private Employee _employee = new Employee();
    private bool _isEditingExistingEmployee = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name must be 50 characters or less")]

    private string _name = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Employee Number is required")]
    [MaxLength(15, ErrorMessage = "Employee number must be 15 characters or less")]
    private string _employeeNo = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(40, ErrorMessage = "Email must be 40 characters or less")]
    private string _email = String.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Department is required")]
    private Department? _selectedDepartment;

    public EmployeeDialogViewModel(INavigationService navigator, DialogManager dialogManager, ToastManager toastManager, IEmployeeService employeeService, IDepartmentService departmentService) : base(navigator)
    {
        Title = "Employee Dialog";
        _employeeService = employeeService;
        _departmentService = departmentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

     [RelayCommand]
    private async Task LoadNewEmployeeAsync()
    {
        Title = "Add Employee";
        _isEditingExistingEmployee = false;
        await InitializeProperties(new Employee());
    }

    [RelayCommand]
    private async Task LoadExistingEmployeeAsync(Employee employee)
    {
        Title = "Edit Employee";
        _isEditingExistingEmployee = true;
        await InitializeProperties(employee);
    }

    [RelayCommand]
    private async Task SaveEmployeeAsync()
    {
        ValidateAllProperties();
        if (HasErrors )
            return;
        
        _employee.Name = Name;
        _employee.Email = Email;
        _employee.EmployeeNo = EmployeeNo;
        _employee.Department = SelectedDepartment;

        try
        {
            if (_isEditingExistingEmployee)
                await _employeeService.UpdateEmployeeAsync(_employee);
            else
                await _employeeService.AddEmployeeAsync(_employee);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        catch (Exception ex)
        {
            var userMessage = BuildSaveErrorMessage(ex);
            _toastManager
                .CreateToast("Unable to save employee")
                .WithContent(userMessage)
                .DismissOnClick()
                .ShowError();
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        finally
        {
            _dialogManager.Dispose();
        }
    }


    private async Task InitializeProperties(Employee employee)
    {
        ClearErrors();

        // Load departments into drop down menu
        IsLoading = true;
        try
        {
            var list = await _departmentService.ReadAllDepartmentsAsync();
            Departments = new ObservableCollection<Department>(list);
        }
        catch (Exception ex)
        {
            Departments = new ObservableCollection<Department>();
            _toastManager
                .CreateToast("Failed to load departments")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
        finally
        {
            IsLoading = false;
        }

        // Observe Employee properties
        _employee = employee;
        Name = employee.Name;
        Email = employee.Email;
        EmployeeNo = employee.EmployeeNo;
        SelectedDepartment = employee.Department;
    }

    private static string BuildSaveErrorMessage(Exception ex)
    {
        if (ex is DbUpdateException { InnerException: SqlException sqlEx } && sqlEx.Number == 2628)
        {
            return "One or more fields are too long. Name max 50, employee number max 15, email max 40.";
        }

        return "Please check required fields and length limits, then try again.";
    }
}
