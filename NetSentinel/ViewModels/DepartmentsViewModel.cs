using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DepartmentsViewModel : ViewModelBase, IRefreshable
{
    private readonly IDepartmentService _departmentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly DepartmentDialogViewModel _departmentDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public DepartmentsViewModel(INavigationService navigator, IDepartmentService departmentService, DialogManager dialogManager, ToastManager toastManager, DepartmentDialogViewModel departmentDialogViewModel) : base(navigator)
    {
        Title = "Departments";
        _departmentService = departmentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _departmentDialogViewModel = departmentDialogViewModel;
    }

    public Task RefreshAsync() => LoadDepartmentsAsync();

    [RelayCommand]
    private async Task LoadDepartmentsAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Departments = new ObservableCollection<Department>();

        try
        {
            var list = await _departmentService.ReadAllDepartmentsAsync();
            Departments = new ObservableCollection<Department>(list);
            IsEmpty = Departments.Count == 0;
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

    [RelayCommand]
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
