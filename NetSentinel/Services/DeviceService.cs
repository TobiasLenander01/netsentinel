using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    // read (all)
    public async Task<List<Device>> ReadAllDevicesAsync()
    {
        return await _dbContext.Devices.ToListAsync();
    }

    // read (single)
    public async Task<Device?> ReadSingleDeviceAsync(int id)
    {
        return await _dbContext.Devices.FindAsync(id);
    }

    // update
    public async Task<int> UpdateDeviceAsync(Device updatedDevice)
    {
        _dbContext.Devices.Update(updatedDevice);
        return await _dbContext.SaveChangesAsync();
    }

    // delete (single)
    public async Task<int> DeleteDevice(Device deviceToDelete)
    {
        _dbContext.Devices.Remove(deviceToDelete);
        return await _dbContext.SaveChangesAsync();
    }
}
