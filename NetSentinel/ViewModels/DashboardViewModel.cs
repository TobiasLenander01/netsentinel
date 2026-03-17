using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using NetSentinel.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ShadUI;

namespace NetSentinel.ViewModels;

public record SecurityMeasureLeaderboardEntry(int Rank, string Name, string Email, int Count);

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IIncidentService _incidentService;
    private readonly IDepartmentService _departmentService;
    private readonly IDeviceService _deviceService;
    private readonly IEmployeeService _employeeService;
    private readonly ISecurityMeasureService _securityMeasureService;
    private readonly ToastManager _toastManager;

    [ObservableProperty]
    private int _departmentCount;

    [ObservableProperty]
    private int _employeeCount;

    [ObservableProperty]
    private int _deviceCount;

    [ObservableProperty]
    private int _incidentCount;

    [ObservableProperty]
    private int _securityMeasureCount;

    [ObservableProperty]
    private ISeries[] _series = [];

    [ObservableProperty]
    private ObservableCollection<SecurityMeasureLeaderboardEntry> _leaderboard = new();

    [ObservableProperty]
    private bool _hasLeaderboard = false;

    [ObservableProperty]
    private bool _isLoading = false;

     public DashboardViewModel(IIncidentService incidentService, IDepartmentService departmentService, IDeviceService deviceService, IEmployeeService employeeService, ISecurityMeasureService securityMeasureService, ToastManager toastManager) : base()
    {
        Title = "Dashboard";
        _incidentService = incidentService;
        _departmentService = departmentService;
        _deviceService = deviceService;
        _employeeService = employeeService;
        _securityMeasureService = securityMeasureService;
        _toastManager = toastManager;

        Series = BuildSeries(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    }


    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;

            var departmentCountTask = _departmentService.ReadDepartmentsCountAsync();
            var employeeCountTask = _employeeService.ReadEmployeesCountAsync();
            var deviceCountTask = _deviceService.ReadDevicesCountAsync();
            var incidentCountTask = _incidentService.ReadIncidentsCountAsync();
            var securityMeasureCountTask = _securityMeasureService.ReadSecurityMeasuresCountAsync();
            var incidentsByMonthTask = _incidentService.ReadIncidentCountsByMonthAsync();
            var leaderboardTask = _securityMeasureService.ReadTopSecurityMeasurePerformersAsync(5);

            await Task.WhenAll(
                departmentCountTask,
                employeeCountTask,
                deviceCountTask,
                incidentCountTask,
                securityMeasureCountTask,
                incidentsByMonthTask,
                leaderboardTask
            );

            DepartmentCount = departmentCountTask.Result;
            EmployeeCount = employeeCountTask.Result;
            DeviceCount = deviceCountTask.Result;
            IncidentCount = incidentCountTask.Result;
            SecurityMeasureCount = securityMeasureCountTask.Result;

            Series = BuildSeries(incidentsByMonthTask.Result);

            var entries = leaderboardTask.Result
                .Select((e, i) => new SecurityMeasureLeaderboardEntry(i + 1, e.Name, e.Email, e.Count));

            Leaderboard = new ObservableCollection<SecurityMeasureLeaderboardEntry>(entries);
            HasLeaderboard = Leaderboard.Count > 0;
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load dashboard")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static ISeries[] BuildSeries(IReadOnlyCollection<int> countsByMonth)
    {
        return
        [
            new ColumnSeries<int>
            {
                Values = countsByMonth,
                Fill = new SolidColorPaint(SKColors.DodgerBlue)
            }
        ];
    }

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
            Labeler = Labelers.Default,
            LabelsPaint = new SolidColorPaint { Color = SKColors.Gray },
            TextSize = 12,
            MinStep = 10,
            ShowSeparatorLines = false
        }
    ];
}