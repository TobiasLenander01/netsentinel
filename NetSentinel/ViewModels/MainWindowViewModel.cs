namespace NetSentinel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public AppNavigator Navigator { get; } = AppNavigator.Instance;
}
