using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    [MaxLength(50, ErrorMessage = "Employee number must be 50 characters or less")]
    private string _EmployeeNumber = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(50, ErrorMessage = "Email must be 50 characters or less")]
    private string _email = String.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Department is required")]
    private Department? _selectedDepartment;

    public EmployeeDialogViewModel(DialogManager dialogManager, ToastManager toastManager, IEmployeeService employeeService, IDepartmentService departmentService) : base()
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
        _employee.EmployeeNumber = EmployeeNumber;
        _employee.Department = SelectedDepartment;

        try
        {
            if (_isEditingExistingEmployee)
                await _employeeService.UpdateEmployeeAsync(_employee);
            else
                await _employeeService.AddEmployeeAsync(_employee);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        catch (InvalidOperationException ex)
        {
            _toastManager
                .CreateToast("Failed to save employee")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        catch
        {
            _toastManager
                .CreateToast("Failed to save employee")
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
        EmployeeNumber = employee.EmployeeNumber;
        SelectedDepartment = employee.Department is null
            ? null
            : Departments.FirstOrDefault(d => d.Id == employee.Department.Id) ?? employee.Department;


        ClearErrors();
    }
}
