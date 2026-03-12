using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DashboardViewModel viewModel)
        {
            viewModel.LoadIncidentsCommand.Execute(null);
        }
    }
}
