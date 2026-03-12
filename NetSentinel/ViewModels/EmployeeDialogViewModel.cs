using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class EmployeeDialogViewModel : ViewModelBase
{
    private readonly EmployeeService _employeeService;
    private readonly DepartmentService _departmentService;
    private readonly DialogManager _dialogManager;

    private Employee _employee = new Employee();
    private bool _isEditingExistingEmployee = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]

    private string _name = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Employee Number is required")]
    private string _employeeNo = String.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Department is required")]
    private Department? _selectedDepartment;

    public EmployeeDialogViewModel(NavigationService navigator, DialogManager dialogManager, EmployeeService employeeService, DepartmentService departmentService) : base(navigator)
    {
        Title = "Employee Dialog";
        _employeeService = employeeService;
        _departmentService = departmentService;
        _dialogManager = dialogManager;
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
        catch
        {
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
        var list = await _departmentService.ReadAllDepartmentsAsync();
        Departments = new ObservableCollection<Department>(list);
        IsLoading = false;

        // Observe Employee properties
        _employee = employee;
        Name = employee.Name;
        EmployeeNo = employee.EmployeeNo;
        SelectedDepartment = employee.Department;
    }
}
