using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public ISeries[] Series { get; } =
    [
        new ColumnSeries<double>
        {
            Values = [2500, 3800, 1500, 4200, 3100, 4800, 2900, 3500, 4100, 2300, 3700, 4500],
            Fill = new SolidColorPaint(SKColors.DodgerBlue)
        }
    ];

    public SolidColorPaint TooltipTextPaint { get; } = new SolidColorPaint { Color = SKColors.Gray };

    public Axis[] XAxes { get; } =
    [
        new Axis
        {
            Labels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            LabelsPaint = new SolidColorPaint { Color = SKColors.Gray },
            TextSize = 12,
            MinStep = 1
        }
    ];

    public Axis[] YAxes { get; } =
    [
        new Axis
        {
            Labeler = Labelers.Currency,
            LabelsPaint = new SolidColorPaint { Color = SKColors.Gray },
            TextSize = 12,
            MinStep = 1500,
            ShowSeparatorLines = false
        }
    ];

    public DashboardViewModel(NavigationService navigator) : base(navigator)
    {
        Title = "Dashboard";
    }
}