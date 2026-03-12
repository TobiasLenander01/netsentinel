using System.Collections.Generic;
using System.Threading.Tasks;
using NetSentinel.Models;

namespace NetSentinel.Interfaces;

public interface ISecurityMeasureService
{
    Task AddSecurityMeasureAsync(SecurityMeasure securityMeasure);
    Task<List<SecurityMeasure>> ReadAllSecurityMeasuresAsync();
    Task<SecurityMeasure?> ReadSingleSecurityMeasureAsync(int id);
    Task<int> UpdateSecurityMeasureAsync(SecurityMeasure updatedSecurityMeasure);
    Task<int> DeleteSecurityMeasure(SecurityMeasure securityMeasureToDelete);
}
