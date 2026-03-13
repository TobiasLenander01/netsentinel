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

    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalCount;

    public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

    public EmployeesViewModel(INavigationService navigator, IEmployeeService employeeService, DialogManager dialogManager, ToastManager toastManager, EmployeeDialogViewModel employeeDialogViewModel) : base(navigator)
    {
        Title = "Employees";
        _employeeService = employeeService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _employeeDialogViewModel = employeeDialogViewModel;
    }

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageInfo));
    partial void OnTotalPagesChanged(int value) => OnPropertyChanged(nameof(PageInfo));

    public Task RefreshAsync() => LoadEmployeesAsync();

    [RelayCommand]
    private async Task LoadEmployeesAsync()
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

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadEmployeesAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadEmployeesAsync();
    }

    private bool CanGoPrevious() => CurrentPage > 1;
    private bool CanGoNext() => CurrentPage < TotalPages;

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
