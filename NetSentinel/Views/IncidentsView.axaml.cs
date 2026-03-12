using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class IncidentsView : UserControl
{
    public IncidentsView()
    {
        InitializeComponent();
    }

      private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IncidentsViewModel viewModel)
        {
            viewModel.LoadIncidentsCommand.Execute(null);
        }
    }
}