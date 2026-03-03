using System;
using System.Threading.Tasks;
using NetSentinel.Database;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class DeviceService
{
    private readonly DatabaseContext _dbContext;

    public DeviceService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    // create 
    public async Task AddDeviceAsync(Device device)
    {
        _dbContext.Devices.Add(device);
        await _dbContext.SaveChangesAsync();
    }
}
