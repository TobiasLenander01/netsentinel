using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class DepartmentsView : UserControl
{
    public DepartmentsView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DepartmentsViewModel viewModel)
        {
            viewModel.LoadDepartmentsCommand.Execute(null);
        }
    }
}
