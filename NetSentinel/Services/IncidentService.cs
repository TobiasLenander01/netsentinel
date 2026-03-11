using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class IncidentService
{
    private readonly DatabaseContext _dbContext;

    public IncidentService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Incident>> ReadAllIncidentsAsync()
    {
        return await _dbContext.Incidents
            .Include(i => i.Device)
            .ToListAsync();
    }
}
