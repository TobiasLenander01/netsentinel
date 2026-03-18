using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetSentinel.Components;

public partial class PaginatedDataGrid : UserControl
{
    public static readonly StyledProperty<object?> DataGridContentProperty =
        AvaloniaProperty.Register<PaginatedDataGrid, object?>(nameof(DataGridContent));

    public object? DataGridContent
    {
        get => GetValue(DataGridContentProperty);
        set => SetValue(DataGridContentProperty, value);
    }
    
    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<PaginatedDataGrid, string>(nameof(EmptyText), "No items found.");

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public PaginatedDataGrid()
    {
        InitializeComponent();
    }
}
