using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class SecurityMeasureService
{
    private readonly DatabaseContext _dbContext;

    public SecurityMeasureService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    // create 
    public async Task AddSecurityMeasureAsync(SecurityMeasure securityMeasure)
    {
        _dbContext.SecurityMeasures.Add(securityMeasure);
        await _dbContext.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<SecurityMeasure>> ReadAllSecurityMeasuresAsync()
    {
        return await _dbContext.SecurityMeasures.Include(sm => sm.Incident).Include(pb => pb.PerformedBy).ToListAsync();
    }

    // read (single)
    public async Task<SecurityMeasure?> ReadSingleSecurityMeasureAsync(int id)
    {
        return await _dbContext.SecurityMeasures.Include(sm => sm.Incident).Include(pb => pb.PerformedBy).FirstOrDefaultAsync(sm => sm.Id == id);
    }

    // update
    public async Task<int> UpdateSecurityMeasureAsync(SecurityMeasure updatedSecurityMeasure)
    {
        _dbContext.SecurityMeasures.Update(updatedSecurityMeasure);
        return await _dbContext.SaveChangesAsync();
    }

    // delete (single)
    public async Task<int> DeleteSecurityMeasure(SecurityMeasure securityMeasureToDelete)
    {
        _dbContext.SecurityMeasures.Remove(securityMeasureToDelete);
        return await _dbContext.SaveChangesAsync();
    }
}
