using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public EmployeeService(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    // create
    public async Task AddEmployeeAsync(Employee employee)
    {
        using var db = _dbContextFactory.CreateDbContext();
        if (employee.Department != null)
            db.Entry(employee.Department).State = EntityState.Unchanged;
        db.Employees.Add(employee);
        await db.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Employee>> ReadAllEmployeesAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Employees
            .Include(e => e.Department)
            .Include(e => e.Devices)
            .ToListAsync();
    }

    // read (paged)
    public async Task<(List<Employee> Items, int TotalCount)> ReadEmployeesPagedAsync(int page, int pageSize)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var query = db.Employees
            .Include(e => e.Department)
            .Include(e => e.Devices)
            .OrderBy(e => e.Name);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    // read (single)
    public async Task<Employee?> ReadSingleEmployeeAsync(int id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return await db.Employees
            .Include(e => e.Department)
            .Include(e => e.Devices)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // update
    public async Task<int> UpdateEmployeeAsync(Employee updatedEmployee)
    {
        using var db = _dbContextFactory.CreateDbContext();
        if (updatedEmployee.Department != null)
            db.Entry(updatedEmployee.Department).State = EntityState.Unchanged;
        db.Employees.Update(updatedEmployee);
        return await db.SaveChangesAsync();
    }

    // delete
    public async Task<int> DeleteEmployeeAsync(Employee employeeToDelete)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.Employees.Remove(employeeToDelete);
        return await db.SaveChangesAsync();
    }
}

