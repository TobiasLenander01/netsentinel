using Avalonia;
using Avalonia.Controls;

namespace NetSentinel.Components;

public partial class OverviewCard : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<OverviewCard, string>(nameof(Title));

    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<OverviewCard, string>(nameof(Value));

    public static readonly StyledProperty<string> HintProperty =
        AvaloniaProperty.Register<OverviewCard, string>(nameof(Hint));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<OverviewCard, object?>(nameof(Icon));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Hint
    {
        get => GetValue(HintProperty);
        set => SetValue(HintProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public OverviewCard()
    {
        InitializeComponent();
    }
}