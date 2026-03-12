using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class DeviceService : IDeviceService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public DeviceService(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    // create 
    public async Task AddDeviceAsync(Device device)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Devices.Add(device);
        await db.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Device>> ReadAllDevicesAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Devices.Include(d => d.Owner).Include(d => d.Incidents).ToListAsync();
    }

    // read (single)
    public async Task<Device?> ReadSingleDeviceAsync(int id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Devices.Include(d => d.Owner).FirstOrDefaultAsync(d => d.Id == id);
    }

    // update
    public async Task<int> UpdateDeviceAsync(Device updatedDevice)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Devices.Update(updatedDevice);
        return await db.SaveChangesAsync();
    }

    // delete (single)
    public async Task<int> DeleteDevice(Device deviceToDelete)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Devices.Remove(deviceToDelete);
        return await db.SaveChangesAsync();
    }
}
