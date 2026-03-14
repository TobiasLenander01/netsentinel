using CommunityToolkit.Mvvm.ComponentModel;

namespace NetSentinel.ViewModels;

/// <summary>
/// The base / superclass for the viewmodels.
/// Each viewmodel inherits properties and methods
/// from this class.
/// </summary>

public partial class ViewModelBase : ObservableValidator
{
    [ObservableProperty]
    private string _title = "Placeholder Title";
}