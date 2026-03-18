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

public partial class EmployeesViewModel : PaginatedViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly EmployeeDialogViewModel _employeeDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    public EmployeesViewModel(IEmployeeService employeeService, DialogManager dialogManager, ToastManager toastManager, EmployeeDialogViewModel employeeDialogViewModel) : base()
    {
        Title = "Employees";
        _employeeService = employeeService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _employeeDialogViewModel = employeeDialogViewModel;
    }

    [RelayCommand]
    private Task LoadEmployeesAsync() => LoadPageAsync();

    protected override async Task LoadPageAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Employees = new ObservableCollection<Employee>();

        try
        {
            var (items, totalCount) = await _employeeService.ReadEmployeesPagedAsync(CurrentPage, PageSize);
            Employees = new ObservableCollection<Employee>(items);
            TotalCount = totalCount;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            IsEmpty = totalCount == 0;
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
            "This will permanently delete this employee. Owned devices will be left without an owner and security measures will remain.")
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
                catch (DbUpdateException)
                {
                    _toastManager
                        .CreateToast("Cannot delete employee")
                        .WithContent("This employee is still referenced by related data. Update or delete those records first.")
                        .DismissOnClick()
                        .ShowError();
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
            .Dismissible()
            .Show();
    }
}
