using CommunityToolkit.Mvvm.ComponentModel;

namespace NetSentinel.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public AppNavigator Navigator { get; } = AppNavigator.Instance;
}
