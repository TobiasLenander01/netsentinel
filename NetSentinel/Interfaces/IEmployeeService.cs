using System.Collections.Generic;
using System.Threading.Tasks;
using NetSentinel.Models;

namespace NetSentinel.Interfaces;

public interface IEmployeeService
{
    Task AddEmployeeAsync(Employee employee);
    Task<List<Employee>> ReadAllEmployeesAsync();
    Task<(List<Employee> Items, int TotalCount)> ReadEmployeesPagedAsync(int page, int pageSize);
    Task<Employee?> ReadSingleEmployeeAsync(int id);
    Task<int> UpdateEmployeeAsync(Employee updatedEmployee);
    Task<int> DeleteEmployeeAsync(Employee employeeToDelete);
}
