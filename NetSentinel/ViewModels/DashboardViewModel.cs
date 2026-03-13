using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using NetSentinel.Services;
using NetSentinel.Interfaces;
using System.Collections.ObjectModel;
using NetSentinel.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IIncidentService _incidentService;
    private readonly ToastManager _toastManager;

    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new ObservableCollection<Incident>();

    [RelayCommand]
    private async Task LoadIncidentsAsync()
    {
        try
        {
            var incidents = await _incidentService.ReadAllIncidentsAsync();
            Incidents = new ObservableCollection<Incident>(incidents);
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load incidents")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

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

    public DashboardViewModel(INavigationService navigator, IIncidentService incidentService, ToastManager toastManager) : base(navigator)
    {
        Title = "Dashboard";
        _incidentService = incidentService;
        _toastManager = toastManager;
    }
}