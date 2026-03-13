using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public DepartmentService(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    // create
    public async Task AddDepartmentAsync(Department department)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Departments.Add(department);
        await db.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Department>> ReadAllDepartmentsAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Departments.Include(d => d.Employees).ToListAsync();
    }

    // read (paged)
    public async Task<(List<Department> Items, int TotalCount)> ReadDepartmentsPagedAsync(int page, int pageSize)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var query = db.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    // read (single)
    public async Task<Department?> ReadSingleDepartmentAsync(int id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);
    }

    // update
    public async Task<int> UpdateDepartmentAsync(Department updatedDepartment)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Departments.Update(updatedDepartment);
        return await db.SaveChangesAsync();
    }

    // delete
    public async Task<int> DeleteDepartmentAsync(Department departmentToDelete)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Departments.Remove(departmentToDelete);
        return await db.SaveChangesAsync();
    }
}
