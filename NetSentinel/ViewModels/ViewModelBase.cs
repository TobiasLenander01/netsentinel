using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

/// <summary>
/// The base / superclass for the viewmodels.
/// Each viewmodel inherits properties and methods
/// from this class.
/// </summary>

public partial class ViewModelBase : ObservableValidator
{
    public INavigationService Navigator { get; }

    [ObservableProperty]
    private string _title = "Placeholder Title";

    public ViewModelBase(INavigationService navigator)
    {
        Navigator = navigator;
    }
}