using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NetSentinel.ViewModels;

namespace NetSentinel.Views;

public partial class DevicesView : UserControl
{
    public DevicesView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DevicesViewModel viewModel)
        {
            viewModel.LoadDevicesCommand.Execute(null);
        }
    }
}