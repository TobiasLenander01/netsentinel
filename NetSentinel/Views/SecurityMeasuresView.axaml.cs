using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class SecurityMeasuresView : UserControl
{
    public SecurityMeasuresView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SecurityMeasuresViewModel viewModel)
        {
            viewModel.LoadSecurityMeasuresCommand.Execute(null);
        }
    }
}
