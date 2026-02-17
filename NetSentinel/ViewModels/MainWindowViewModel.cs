using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // The View binds to this property
    public IAppNavigator Navigator { get; }

    // Injected by DI
    public MainWindowViewModel(IAppNavigator navigator)
    {
        Navigator = navigator;
    }
}
