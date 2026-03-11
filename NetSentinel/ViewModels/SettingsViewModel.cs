using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Database;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly DatabaseContext _db;
    private readonly ToastManager _toastManager;

    public SettingsViewModel(NavigationService navigator, DatabaseContext db, ToastManager toastManager) : base(navigator)
    {
        Title = "Settings";
        _db = db;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task SeedDatabaseAsync()
    {
        await _db.SeedAsync();
        _toastManager.CreateToast("Database seeded").WithContent("Test data has been inserted.").DismissOnClick().ShowSuccess();
    }

    [RelayCommand]
    private async Task ClearDatabaseAsync()
    {
        await _db.ClearAllAsync();
        _toastManager.CreateToast("Database cleared").WithContent("All rows have been removed.").DismissOnClick().Show();
    }
}
