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

public partial class DepartmentsViewModel : ViewModelBase, IRefreshable
{
    private readonly IDepartmentService _departmentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly DepartmentDialogViewModel _departmentDialogViewModel;

    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

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

    public DepartmentsViewModel(INavigationService navigator, IDepartmentService departmentService, DialogManager dialogManager, ToastManager toastManager, DepartmentDialogViewModel departmentDialogViewModel) : base(navigator)
    {
        Title = "Departments";
        _departmentService = departmentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _departmentDialogViewModel = departmentDialogViewModel;
    }

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageInfo));
    partial void OnTotalPagesChanged(int value) => OnPropertyChanged(nameof(PageInfo));

    public Task RefreshAsync() => LoadDepartmentsAsync();

    [RelayCommand]
    private async Task LoadDepartmentsAsync()
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

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadDepartmentsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadDepartmentsAsync();
    }

    private bool CanGoPrevious() => CurrentPage > 1;
    private bool CanGoNext() => CurrentPage < TotalPages;

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
            .WithMaxWidth(512)
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
