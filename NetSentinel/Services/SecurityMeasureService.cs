using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class SecurityMeasureService : ISecurityMeasureService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public SecurityMeasureService(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    // create 
    public async Task AddSecurityMeasureAsync(SecurityMeasure securityMeasure)
    {
        using var db = _dbContextFactory.CreateDbContext();
        if (securityMeasure.Incident != null)
            db.Entry(securityMeasure.Incident).State = EntityState.Unchanged;
        if (securityMeasure.PerformedBy != null)
            db.Entry(securityMeasure.PerformedBy).State = EntityState.Unchanged;
        db.SecurityMeasures.Add(securityMeasure);
        await db.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<SecurityMeasure>> ReadAllSecurityMeasuresAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.SecurityMeasures.Include(sm => sm.Incident).Include(pb => pb.PerformedBy).ToListAsync();
    }

    // read (single)
    public async Task<SecurityMeasure?> ReadSingleSecurityMeasureAsync(int id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.SecurityMeasures.Include(sm => sm.Incident).Include(pb => pb.PerformedBy).FirstOrDefaultAsync(sm => sm.Id == id);
    }

    // update
    public async Task<int> UpdateSecurityMeasureAsync(SecurityMeasure updatedSecurityMeasure)
    {
        using var db = _dbContextFactory.CreateDbContext();
        if (updatedSecurityMeasure.Incident != null)
            db.Entry(updatedSecurityMeasure.Incident).State = EntityState.Unchanged;
        if (updatedSecurityMeasure.PerformedBy != null)
            db.Entry(updatedSecurityMeasure.PerformedBy).State = EntityState.Unchanged;
        db.SecurityMeasures.Update(updatedSecurityMeasure);
        return await db.SaveChangesAsync();
    }

    // delete (single)
    public async Task<int> DeleteSecurityMeasure(SecurityMeasure securityMeasureToDelete)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.SecurityMeasures.Remove(securityMeasureToDelete);
        return await db.SaveChangesAsync();
    }
}
