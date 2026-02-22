using System;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(INavigation navigator) : base(navigator)
    {
        Title = "Settings";
    }
}
