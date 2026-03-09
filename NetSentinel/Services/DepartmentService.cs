using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class DepartmentService
{
    private readonly DatabaseContext _dbContext;

    public DepartmentService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    // create
    public async Task AddDepartmentAsync(Department department)
    {
        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Department>> ReadAllDepartmentsAsync()
    {
        return await _dbContext.Departments.Include(d => d.Employees).ToListAsync();
    }

    // read (single)
    public async Task<Department?> ReadSingleDepartmentAsync(int id)
    {
        return await _dbContext.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);
    }

    // update
    public async Task<int> UpdateDepartmentAsync(Department updatedDepartment)
    {
        _dbContext.Departments.Update(updatedDepartment);
        return await _dbContext.SaveChangesAsync();
    }

    // delete
    public async Task<int> DeleteDepartmentAsync(Department departmentToDelete)
    {
        _dbContext.Departments.Remove(departmentToDelete);
        return await _dbContext.SaveChangesAsync();
    }
}
