using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    // Accessible by any class that inherits from ViewModelBase
    public IAppNavigator Navigator { get; }

    public virtual string Title => "NetSentinel";

    public ViewModelBase(IAppNavigator navigator)
    {
        Navigator = navigator;
    }
}