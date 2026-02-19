using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    public IAppNavigator Navigator { get; }

    [ObservableProperty]
    private string _title = "Placeholder Title";

    public ViewModelBase(IAppNavigator navigator)
    {
        Navigator = navigator;
    }
}