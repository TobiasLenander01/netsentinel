using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Database;
using NetSentinel.Models;

namespace NetSentinel.Services;

public class EmployeeService
{
    private readonly DatabaseContext _dbContext;

    public EmployeeService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    // create
    public async Task AddEmployeeAsync(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
    }

    // read (all)
    public async Task<List<Employee>> ReadAllEmployeesAsync()
    {
        return await _dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Devices)
            .ToListAsync();
    }

    // read (single)
    public async Task<Employee?> ReadSingleEmployeeAsync(int id)
    {
        return await _dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Devices)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // update
    public async Task<int> UpdateEmployeeAsync(Employee updatedEmployee)
    {
        _dbContext.Employees.Update(updatedEmployee);
        return await _dbContext.SaveChangesAsync();
    }

    // delete
    // delete
    public async Task<int> DeleteEmployeeAsync(Employee employeeToDelete)
    {
        _dbContext.Employees.Remove(employeeToDelete);
        return await _dbContext.SaveChangesAsync();
    }
}

