
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class DevicesViewModel : ViewModelBase
{
    private readonly DeviceService _deviceService;

    public DevicesViewModel(NavigationService navigator, DeviceService deviceService) : base(navigator)
    {
        Title = "Devices";
        _deviceService = deviceService;
    }
}