using Avalonia;
using Avalonia.Controls;
using IncidentPriority = NetSentinel.Enums.Priority;

namespace NetSentinel.Components;

public partial class Priority : UserControl
{
    public static readonly StyledProperty<IncidentPriority> ValueProperty =
        AvaloniaProperty.Register<Priority, IncidentPriority>(nameof(Value));

    public static readonly DirectProperty<Priority, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<Priority, string>(
            nameof(DisplayText),
            o => o.DisplayText);

    private string _displayText = string.Empty;

    public IncidentPriority Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string DisplayText
    {
        get => _displayText;
        private set => SetAndRaise(DisplayTextProperty, ref _displayText, value);
    }

    public Priority()
    {
        InitializeComponent();
        UpdateVisualState(Value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty && change.NewValue is IncidentPriority priority)
        {
            UpdateVisualState(priority);
        }
    }

    private void UpdateVisualState(IncidentPriority priority)
    {
        var className = priority switch
        {
            IncidentPriority.Low => "priority-low",
            IncidentPriority.Medium => "priority-medium",
            IncidentPriority.High => "priority-high",
            IncidentPriority.Critical => "priority-critical",
            _ => string.Empty
        };

        Badge.Classes.Remove("priority-low");
        Badge.Classes.Remove("priority-medium");
        Badge.Classes.Remove("priority-high");
        Badge.Classes.Remove("priority-critical");

        Label.Classes.Remove("priority-low");
        Label.Classes.Remove("priority-medium");
        Label.Classes.Remove("priority-high");
        Label.Classes.Remove("priority-critical");

        if (!string.IsNullOrEmpty(className))
        {
            Badge.Classes.Add(className);
            Label.Classes.Add(className);
        }

        DisplayText = priority.ToString();
    }
}