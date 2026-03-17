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

            var departmentCount = await _departmentService.ReadDepartmentsCountAsync();
            var employeeCount = await _employeeService.ReadEmployeesCountAsync();
            var deviceCount = await _deviceService.ReadDevicesCountAsync();
            var incidentCount = await _incidentService.ReadIncidentsCountAsync();
            var securityMeasureCount = await _securityMeasureService.ReadSecurityMeasuresCountAsync();
            var incidentsByMonth = await _incidentService.ReadIncidentCountsByMonthAsync();
            var leaderboard = await _securityMeasureService.ReadTopSecurityMeasurePerformersAsync(5);

            DepartmentCount = departmentCount;
            EmployeeCount = employeeCount;
            DeviceCount = deviceCount;
            IncidentCount = incidentCount;
            SecurityMeasureCount = securityMeasureCount;

            Series = BuildSeries(incidentsByMonth);

            var entries = leaderboard
                .Select((employee, index) => new SecurityMeasureLeaderboardEntry(index + 1, employee.Name, employee.Email, employee.Count));

            Leaderboard = new ObservableCollection<SecurityMeasureLeaderboardEntry>(entries);
            HasLeaderboard = Leaderboard.Count > 0;
        }
        catch
        {
            _toastManager
                .CreateToast("Failed to load dashboard")
                .WithContent("Verify your network connection and try again.")
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