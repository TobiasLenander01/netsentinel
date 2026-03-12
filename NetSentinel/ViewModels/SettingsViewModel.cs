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
        bool seeded = await _db.SeedAsync();
        if (seeded)
            _toastManager.CreateToast("Database seeded").WithContent("Test data has been inserted.").DismissOnClick().ShowSuccess();
        else
            _toastManager.CreateToast("Database is not empty").WithContent("Clear the database first before seeding.").DismissOnClick().ShowWarning();
    }

    [RelayCommand]
    private async Task ClearDatabaseAsync()
    {
        await _db.ClearAllAsync();
        _toastManager.CreateToast("Database cleared").WithContent("All rows have been removed.").DismissOnClick().Show();
    }
}
