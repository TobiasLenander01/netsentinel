using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class DepartmentsViewModel : ViewModelBase
{
    private readonly DepartmentService _departmentService;

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public DepartmentsViewModel(NavigationService navigator, DepartmentService departmentService) : base(navigator)
    {
        Title = "Departments";
        _departmentService = departmentService;
    }

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
}
