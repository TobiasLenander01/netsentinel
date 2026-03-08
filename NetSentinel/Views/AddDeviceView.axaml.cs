using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class AddDeviceView : UserControl
{
    public AddDeviceView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddDeviceViewModel viewModel)
        {
            viewModel.LoadCommand.Execute(null);
        }
    }
}