using System.Collections.Generic;
using System.Threading.Tasks;
using NetSentinel.Models;

namespace NetSentinel.Interfaces;

public interface IIncidentService
{
    public Task AddIncidentAsync(Incident incident);
    public Task<List<Incident>> ReadAllIncidentsAsync();
    public Task<(List<Incident> Items, int TotalCount)> ReadIncidentsPagedAsync(int page, int pageSize);
    public Task<Incident?> ReadSingleIncidentAsync(int id);
    public Task<int> UpdateIncidentAsync(Incident updatedIncident);
    public Task<int> DeleteIncidentAsync(Incident incidentToDelete);
}
