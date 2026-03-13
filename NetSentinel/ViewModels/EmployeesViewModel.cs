using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class EmployeesViewModel : ViewModelBase, IRefreshable
{
    private readonly IEmployeeService _employeeService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly EmployeeDialogViewModel _employeeDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public EmployeesViewModel(INavigationService navigator, IEmployeeService employeeService, DialogManager dialogManager, ToastManager toastManager, EmployeeDialogViewModel employeeDialogViewModel) : base(navigator)
    {
        Title = "Employees";
        _employeeService = employeeService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _employeeDialogViewModel = employeeDialogViewModel;
    }

    public Task RefreshAsync() => LoadEmployeesAsync();

    [RelayCommand]
    private async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Employees = new ObservableCollection<Employee>();

        try
        {
            var employeeList = await _employeeService.ReadAllEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employeeList);
            IsEmpty = Employees.Count == 0;
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load employees")
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
    public async Task ShowNewEmployeeDialog()
    {
        try
        {
            await _employeeDialogViewModel.LoadNewEmployeeCommand.ExecuteAsync(null);
            _dialogManager.CreateDialog(_employeeDialogViewModel)
                .Dismissible()
                .WithSuccessCallback(() =>
                {
                    _toastManager
                        .CreateToast("Employee added")
                        .DismissOnClick()
                        .ShowSuccess();
                    _ = LoadEmployeesAsync();
                })
                .Show();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open new employee")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public async Task ShowEditEmployeeDialog(Employee employee)
    {
        try
        {
            await _employeeDialogViewModel.LoadExistingEmployeeCommand.ExecuteAsync(employee);
            _dialogManager.CreateDialog(_employeeDialogViewModel)
                .Dismissible()
                .WithSuccessCallback(() =>
                {
                    _toastManager
                        .CreateToast("Employee edited")
                        .DismissOnClick()
                        .ShowSuccess();
                    _ = LoadEmployeesAsync();
                })
                .Show();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open employee")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public void ShowDeleteEmployeeDialog(Employee employee)
    {
        _dialogManager.CreateDialog(
            "Are you absolutely sure?",
            "This will permanently delete this employee.")
            .WithPrimaryButton("Confirm", async () =>
            {
                try
                {
                    await _employeeService.DeleteEmployeeAsync(employee);
                    _toastManager.CreateToast("Employee deleted")
                        .WithContent("Employee deleted successfully!")
                        .DismissOnClick()
                        .ShowSuccess();
                    await LoadEmployeesAsync();
                }
                catch (Exception ex)
                {
                    _toastManager
                        .CreateToast("Failed to delete employee")
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
