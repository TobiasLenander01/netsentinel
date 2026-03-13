using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly DatabaseContext _db;
    private readonly ToastManager _toastManager;

    public SettingsViewModel(INavigationService navigator, DatabaseContext db, ToastManager toastManager) : base(navigator)
    {
        Title = "Settings";
        _db = db;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task SeedDatabaseAsync()
    {
        try
        {
            bool seeded = await _db.SeedAsync();
            if (seeded)
                _toastManager.CreateToast("Database seeded").WithContent("Test data has been inserted.").DismissOnClick().ShowSuccess();
            else
                _toastManager.CreateToast("Database is not empty").WithContent("Clear the database first before seeding.").DismissOnClick().ShowWarning();
        }
        catch (System.Exception ex)
        {
            _toastManager.CreateToast("Failed to seed database").WithContent(ex.Message).DismissOnClick().ShowError();
        }
    }

    [RelayCommand]
    private async Task ClearDatabaseAsync()
    {
        try
        {
            await _db.ClearAllAsync();
            _toastManager.CreateToast("Database cleared").WithContent("All rows have been removed.").DismissOnClick().ShowSuccess();
        }
        catch (System.Exception ex)
        {
            _toastManager.CreateToast("Failed to clear database").WithContent(ex.Message).DismissOnClick().ShowError();
        }
    }
}
