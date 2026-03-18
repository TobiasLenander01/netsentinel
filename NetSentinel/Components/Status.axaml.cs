using Avalonia;
using Avalonia.Controls;
using IncidentStatus = NetSentinel.Enums.Status;

namespace NetSentinel.Components;

public partial class Status : UserControl
{
    public static readonly StyledProperty<IncidentStatus> ValueProperty =
        AvaloniaProperty.Register<Status, IncidentStatus>(nameof(Value));

    public static readonly DirectProperty<Status, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<Status, string>(
            nameof(DisplayText),
            o => o.DisplayText);

    private string _displayText = string.Empty;

    public IncidentStatus Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string DisplayText
    {
        get => _displayText;
        private set => SetAndRaise(DisplayTextProperty, ref _displayText, value);
    }

    public Status()
    {
        InitializeComponent();
        UpdateVisualState(Value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty && change.NewValue is IncidentStatus status)
        {
            UpdateVisualState(status);
        }
    }

    private void UpdateVisualState(IncidentStatus status)
    {
        var className = status switch
        {
            IncidentStatus.Approved => "status-approved",
            IncidentStatus.Rejected => "status-rejected",
            IncidentStatus.InProgress => "status-inprogress",
            IncidentStatus.Completed => "status-completed",
            _ => string.Empty
        };

        Badge.Classes.Remove("status-approved");
        Badge.Classes.Remove("status-rejected");
        Badge.Classes.Remove("status-inprogress");
        Badge.Classes.Remove("status-completed");

        Label.Classes.Remove("status-approved");
        Label.Classes.Remove("status-rejected");
        Label.Classes.Remove("status-inprogress");
        Label.Classes.Remove("status-completed");

        if (!string.IsNullOrEmpty(className))
        {
            Badge.Classes.Add(className);
            Label.Classes.Add(className);
        }

        DisplayText = status switch
        {
            IncidentStatus.InProgress => "In Progress",
            _ => status.ToString()
        };
    }
}