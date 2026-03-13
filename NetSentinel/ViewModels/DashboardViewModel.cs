using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using NetSentinel.Interfaces;
using System.Collections.ObjectModel;
using NetSentinel.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IIncidentService _incidentService;
    private readonly IDepartmentService _departmentService;
    private readonly IDeviceService _deviceService;
    private readonly IEmployeeService _employeeService;
    private readonly ISecurityMeasureService _securityMeasureService;


    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new ObservableCollection<Incident>();

    [ObservableProperty]
    private ObservableCollection<Device> _devices = new ObservableCollection<Device>();

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new ObservableCollection<Employee>();

    [ObservableProperty]
    private ObservableCollection<SecurityMeasure> _securityMeasures = new ObservableCollection<SecurityMeasure>();

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new ObservableCollection<Department>();

    [ObservableProperty]
    private ISeries[] _series = [];

     public DashboardViewModel(INavigationService navigator, IIncidentService incidentService, IDepartmentService departmentService, IDeviceService deviceService, IEmployeeService employeeService, ISecurityMeasureService securityMeasureService) : base(navigator)
    {
        Title = "Dashboard";
        _incidentService = incidentService;
        _departmentService = departmentService;
        _deviceService = deviceService;
        _employeeService = employeeService;
        _securityMeasureService = securityMeasureService;
        UpdateSeries();
    }


    [RelayCommand]
    private async Task LoadDepartmentsAsync()
    {
        var departments = await _departmentService.ReadAllDepartmentsAsync();
        Departments = new ObservableCollection<Department>(departments);
    }

    [RelayCommand]  
    private async Task LoadDevicesAsync()
    {
        var devices = await _deviceService.ReadAllDevicesAsync();
        Devices = new ObservableCollection<Device>(devices);
    }

    [RelayCommand]
    private async Task LoadEmployeesAsync()
    {
        var employees = await _employeeService.ReadAllEmployeesAsync();
        Employees = new ObservableCollection<Employee>(employees);
    }

    [RelayCommand]
    private async Task LoadSecurityMeasuresAsync()
    {
        var securityMeasures = await _securityMeasureService.ReadAllSecurityMeasuresAsync();
        SecurityMeasures = new ObservableCollection<SecurityMeasure>(securityMeasures);
    }

    [RelayCommand]
    private async Task LoadIncidentsAsync()
    {
        var incidents = await _incidentService.ReadAllIncidentsAsync();
        Incidents = new ObservableCollection<Incident>(incidents);
    }

    partial void OnIncidentsChanged(ObservableCollection<Incident> value)
    {
        UpdateSeries();
    }

    private List<int> IncidentsPerMonth(ObservableCollection<Incident> incidents)
    {
        var counts = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // One count for each month

        foreach (var incident in incidents)
        {
            // Logic to count incidents per month and update the counts list
                int month = incident.CreatedAt.Month; // Get the month from the CreatedAt property
                counts[month - 1]++; // Increment the count for the corresponding month (month - 1 because of zero-based index)

        }

        return counts;
    }

    private void UpdateSeries()
    {
        Series =
        [
            new ColumnSeries<int>
            {
                Values = IncidentsPerMonth(Incidents),
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