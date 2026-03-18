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

public partial class DepartmentsViewModel : PaginatedViewModelBase
{
    private readonly IDepartmentService _departmentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly DepartmentDialogViewModel _departmentDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    public DepartmentsViewModel(IDepartmentService departmentService, DialogManager dialogManager, ToastManager toastManager, DepartmentDialogViewModel departmentDialogViewModel) : base()
    {
        Title = "Departments";
        _departmentService = departmentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _departmentDialogViewModel = departmentDialogViewModel;
    }

    [RelayCommand]
    private Task LoadDepartmentsAsync() => LoadPageAsync();

    protected override async Task LoadPageAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Departments = new ObservableCollection<Department>();

        try
        {
            var (items, totalCount) = await _departmentService.ReadDepartmentsPagedAsync(CurrentPage, PageSize);
            Departments = new ObservableCollection<Department>(items);
            TotalCount = totalCount;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            IsEmpty = totalCount == 0;
        }
        catch (Exception ex)
        {
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
    }

    private async Task DeleteDepartmentAsync(Department department)
    {
        try
        {
            await _departmentService.DeleteDepartmentAsync(department);
            Departments.Remove(department);
            IsEmpty = Departments.Count == 0;
            _toastManager
                .CreateToast("Department deleted")
                .DismissOnClick()
                .ShowSuccess();
        }
        catch (DbUpdateException)
        {
            _toastManager
                .CreateToast("Cannot delete department")
                .WithContent("This department still has employees")
                .DismissOnClick()
                .ShowError();
        }
        catch (InvalidOperationException ex)
        {
            _toastManager
                .CreateToast("Cannot delete department")
                .WithContent("This department still has employees")
                .DismissOnClick()
                .ShowError();
            Console.WriteLine(ex);
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to delete department")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public void ShowDeleteDepartmentDialog(Department department)
    {
        _dialogManager.CreateDialog(
            "Are you absolutely sure?",
            "This will permanently delete this department.")
            .WithPrimaryButton("Confirm", async () =>
            {
                await DeleteDepartmentAsync(department);
            }, DialogButtonStyle.Destructive)
            .WithCancelButton("Cancel")
            .Dismissible()
            .Show();
    }

    [RelayCommand]
    public async Task ShowNewDepartmentDialog()
    {
        try
        {
            await _departmentDialogViewModel.LoadNewDepartmentCommand.ExecuteAsync(null);
            _dialogManager.CreateDialog(_departmentDialogViewModel)
            .Dismissible()
            .WithSuccessCallback(() =>
                {
                    _toastManager
                        .CreateToast("Department added")
                        .DismissOnClick()
                        .ShowSuccess();
                    _ = LoadDepartmentsAsync();
                })
            .Show();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open new department")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public async Task ShowEditDepartmentDialog(Department department)
    {
        try
        {
            await _departmentDialogViewModel.LoadExistingDepartmentCommand.ExecuteAsync(department);
            _dialogManager.CreateDialog(_departmentDialogViewModel)
            .Dismissible()
            .WithSuccessCallback(() =>
                {
                    _toastManager
                        .CreateToast("Department edited")
                        .DismissOnClick()
                        .ShowSuccess();
                    _ = LoadDepartmentsAsync();
                })
            .Show();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open department")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }
}
