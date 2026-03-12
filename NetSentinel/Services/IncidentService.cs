using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class IncidentService : IIncidentService
{
    private readonly DatabaseContext _dbContext;

    public IncidentService(DatabaseContext dbContext) 
    {
        _dbContext = dbContext;
    }

    // create
    public async Task AddIncidentAsync(Incident incident)
    {
        _dbContext.Incidents.Add(incident);
        await _dbContext.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Incident>> ReadAllIncidentsAsync()
    {
        return await _dbContext.Incidents
            .Include(i=> i.Device)
            .Include(i=> i.SecurityMeasures)
            .ToListAsync();
    }

    // read (single)
    public async Task<Incident?> ReadSingleIncidentAsync(int id)
    {
        return await _dbContext.Incidents
            .Include(i=> i.Device)
            .Include(i=> i.SecurityMeasures)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    // update
    public async Task<int> UpdateIncidentAsync(Incident updatedIncident)
    {
        _dbContext.Incidents.Update(updatedIncident);
        return await _dbContext.SaveChangesAsync();
    }

    // delete
    public async Task<int> DeleteIncidentAsync(Incident incidentToDelete)
    {
        _dbContext.Incidents.Remove(incidentToDelete);
        return await _dbContext.SaveChangesAsync();
    }

}
