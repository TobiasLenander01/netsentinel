using System.Collections.Generic;
using System.Threading.Tasks;
using NetSentinel.Models;

namespace NetSentinel.Interfaces;

public interface IDeviceService
{
    Task AddDeviceAsync(Device device);
    Task<List<Device>> ReadAllDevicesAsync();
    Task<(List<Device> Items, int TotalCount)> ReadDevicesPagedAsync(int page, int pageSize);
    Task<Device?> ReadSingleDeviceAsync(int id);
    Task<int> UpdateDeviceAsync(Device updatedDevice);
    Task<int> DeleteDevice(Device deviceToDelete);
}
