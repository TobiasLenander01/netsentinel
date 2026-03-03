using System;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(NavigationService navigator) : base(navigator)
    {
        Title = "Settings";
    }
}
