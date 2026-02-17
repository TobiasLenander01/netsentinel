using System;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Settings";

    public SettingsViewModel(IAppNavigator navigator) : base(navigator)
    {
        Title = _title;
    }
}
