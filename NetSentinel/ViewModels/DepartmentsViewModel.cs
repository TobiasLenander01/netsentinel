using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DepartmentsViewModel : ViewModelBase, IRefreshable
{
    private readonly DepartmentService _departmentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly DepartmentDialogViewModel _departmentDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public DepartmentsViewModel(NavigationService navigator, DepartmentService departmentService, DialogManager dialogManager, ToastManager toastManager, DepartmentDialogViewModel departmentDialogViewModel) : base(navigator)
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
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteDepartmentAsync(Department department)
    {
        await _departmentService.DeleteDepartmentAsync(department);
        Departments.Remove(department);
        IsEmpty = Departments.Count == 0;
    }

    [RelayCommand]
    public async Task ShowNewDepartmentDialog()
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

    [RelayCommand]
    public async Task ShowEditDepartmentDialog(Department department)
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
}
