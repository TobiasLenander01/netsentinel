using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class IncidentService : IIncidentService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public IncidentService(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    // create
    public async Task AddIncidentAsync(Incident incident)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Incidents.Add(incident);
        await db.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Incident>> ReadAllIncidentsAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Incidents
            .Include(i=> i.Device)
            .Include(i=> i.SecurityMeasures)
            .ToListAsync();
    }

    // read (single)
    public async Task<Incident?> ReadSingleIncidentAsync(int id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Incidents
            .Include(i=> i.Device)
            .Include(i=> i.SecurityMeasures)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    // update
    public async Task<int> UpdateIncidentAsync(Incident updatedIncident)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Incidents.Update(updatedIncident);
        return await db.SaveChangesAsync();
    }

    // delete
    public async Task<int> DeleteIncidentAsync(Incident incidentToDelete)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Incidents.Remove(incidentToDelete);
        return await db.SaveChangesAsync();
    }

}
