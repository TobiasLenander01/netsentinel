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
        incident.CreatedAt = NormalizeToUtc(incident.CreatedAt);
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

        var now = System.DateTime.UtcNow;
        var start = new System.DateTime(now.Year, now.Month, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMonths(-11);
        var end = start.AddMonths(12);

        var monthly = await db.Incidents
            .AsNoTracking()
            .Where(i => i.CreatedAt >= start && i.CreatedAt < end)
            .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();

        var monthlyLookup = monthly.ToDictionary(
            entry => (entry.Year, entry.Month),
            entry => entry.Count);

        var counts = new List<int>(12);
        for (var i = 0; i < 12; i++)
        {
            var month = start.AddMonths(i);
            counts.Add(monthlyLookup.TryGetValue((month.Year, month.Month), out var count) ? count : 0);
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
        updatedIncident.CreatedAt = NormalizeToUtc(updatedIncident.CreatedAt);
        if (updatedIncident.Device != null)
            db.Entry(updatedIncident.Device).State = EntityState.Unchanged;
        db.Incidents.Update(updatedIncident);
        return await db.SaveChangesAsync();
    }

    private static System.DateTime NormalizeToUtc(System.DateTime value)
    {
        return value.Kind switch
        {
            System.DateTimeKind.Utc => value,
            System.DateTimeKind.Local => value.ToUniversalTime(),
            _ => value
        };
    }

    // delete
    public async Task<int> DeleteIncidentAsync(Incident incidentToDelete)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Incidents.Remove(incidentToDelete);
        return await db.SaveChangesAsync();
    }

}
