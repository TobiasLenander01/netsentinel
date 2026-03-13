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

    [RelayCommand]
    private async Task DeleteDepartmentAsync(Department department)
    {
        await _departmentService.DeleteDepartmentAsync(department);
        await LoadDepartmentsAsync();
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
