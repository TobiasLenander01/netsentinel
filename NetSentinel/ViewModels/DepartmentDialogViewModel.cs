using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DepartmentDialogViewModel : ViewModelBase
{
    private readonly IDepartmentService _departmentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    private Department _department = new Department();
    private bool _isEditingExistingDepartment = false;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name must be 50 characters or less")]
    private string _name = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Location is required")]
    [MaxLength(50, ErrorMessage = "Location must be 50 characters or less")]
    private string _location = String.Empty;

    public DepartmentDialogViewModel(DialogManager dialogManager, ToastManager toastManager, IDepartmentService departmentService) : base()
    {
        Title = "Department Dialog";
        _departmentService = departmentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private Task LoadNewDepartmentAsync()
    {
        Title = "Add Department";
        _isEditingExistingDepartment = false;
        InitializeProperties(new Department());
        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task LoadExistingDepartmentAsync(Department department)
    {
        Title = "Edit Department";
        _isEditingExistingDepartment = true;
        InitializeProperties(department);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SaveDepartmentAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
            return;

        _department.Name = Name;
        _department.Location = Location;

        try
        {
            if (_isEditingExistingDepartment)
                await _departmentService.UpdateDepartmentAsync(_department);
            else
                await _departmentService.AddDepartmentAsync(_department);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        catch
        {
            _toastManager
                .CreateToast("Failed to save department")
                .DismissOnClick()
                .ShowError();
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        finally
        {
            _dialogManager.Dispose();
        }
    }

    private void InitializeProperties(Department department)
    {
        _department = department;
        Name = department.Name;
        Location = department.Location;

        ClearErrors();
    }
}
