using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class EmployeesView : UserControl
{
    public EmployeesView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is EmployeesViewModel viewModel)
        {
            viewModel.LoadEmployeesCommand.Execute(null);
        }
    }
}
