using System.Collections.Generic;
using System.Linq;
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
        if (incident.Device != null)
            db.Entry(incident.Device).State = EntityState.Unchanged;
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

    // read (count)
    public async Task<int> ReadIncidentsCountAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Incidents.AsNoTracking().CountAsync();
    }

    // read (counts by month)
    public async Task<List<int>> ReadIncidentCountsByMonthAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var monthly = await db.Incidents
            .AsNoTracking()
            .GroupBy(i => i.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync();

        var counts = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        foreach (var entry in monthly)
        {
            if (entry.Month >= 1 && entry.Month <= 12)
            {
                counts[entry.Month - 1] = entry.Count;
            }
        }

        return counts;
    }

    // read (paged)
    public async Task<(List<Incident> Items, int TotalCount)> ReadIncidentsPagedAsync(int page, int pageSize)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var query = db.Incidents
            .Include(i => i.Device)
            .Include(i => i.SecurityMeasures)
            .OrderByDescending(i => i.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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
        if (updatedIncident.Device != null)
            db.Entry(updatedIncident.Device).State = EntityState.Unchanged;
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
