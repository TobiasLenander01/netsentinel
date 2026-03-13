using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetSentinel.Models;

namespace NetSentinel.Interfaces;

public interface IDepartmentService
{
    public Task AddDepartmentAsync(Department department);
    public Task<List<Department>> ReadAllDepartmentsAsync();
    public Task<(List<Department> Items, int TotalCount)> ReadDepartmentsPagedAsync(int page, int pageSize);
    public Task<Department?> ReadSingleDepartmentAsync(int id);
    public Task<int> UpdateDepartmentAsync(Department updatedDepartment);
    public Task<int> DeleteDepartmentAsync(Department departmentToDelete);
}
