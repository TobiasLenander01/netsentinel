using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Enums;
using NetSentinel.Models;

namespace NetSentinel.Database;

/// <summary>
/// AUTOMATIC MIGRATIONS
/// 1. Install EF Core Tools
/// dotnet tool install --global dotnet-ef
/// 2. Create migration
/// dotnet ef migrations add InitialCreate
/// 3. Apply migration to database
/// dotnet ef database update
/// </summary>

public class DatabaseContext : DbContext
{
    // define database tables
    public DbSet<Device> Devices {get; set;}
    public DbSet<Employee> Employees {get; set;}
    public DbSet<Incident> Incidents {get; set;}
    public DbSet<SecurityMeasure> SecurityMeasures {get; set;}
    public DbSet<Department> Departments {get; set;}


    public DatabaseContext (DbContextOptions<DatabaseContext> options) : base(options) {}

    /// <summary>
    /// Clears all tables in FK-safe order (children before parents).
    /// </summary>
    public async Task ClearAllAsync()
    {
        SecurityMeasures.RemoveRange(SecurityMeasures);
        Incidents.RemoveRange(Incidents);
        Devices.RemoveRange(Devices);
        Employees.RemoveRange(Employees);
        Departments.RemoveRange(Departments);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Clears all tables and inserts fresh test data.
    /// Deletion order respects FK constraints (children before parents).
    /// </summary>
    public async Task SeedAsync()
    {
        await ClearAllAsync();

        // --- Departments ---
        var deptIT = new Department("IT", "Floor 2");
        var deptHR = new Department("HR", "Floor 1");
        var deptFinance = new Department("Finance", "Floor 3");
        Departments.AddRange(deptIT, deptHR, deptFinance);

        // --- Employees ---
        var emp1 = new Employee("EMP001", "Alice Berg")   { Department = deptIT };
        var emp2 = new Employee("EMP002", "Bob Lindqvist"){ Department = deptIT };
        var emp3 = new Employee("EMP003", "Carla Mäkinen"){ Department = deptHR };
        var emp4 = new Employee("EMP004", "David Holm")   { Department = deptFinance };
        Employees.AddRange(emp1, emp2, emp3, emp4);

        // --- Devices ---
        var dev1 = new Device { Name = "Macbook",  IPAddress = "192.168.1.10", SerialNumber = "SN-AA-0001", Owner = emp1 };
        var dev2 = new Device { Name = "Windows PC",   IPAddress = "192.168.1.11", SerialNumber = "SN-BB-0002", Owner = emp2 };
        var dev3 = new Device { Name = "Chromebook",  IPAddress = "192.168.1.20", SerialNumber = "SN-CC-0003", Owner = emp3 };
        var dev4 = new Device { Name = "Workstation", IPAddress = "192.168.1.30", SerialNumber = "SN-DD-0004", Owner = emp4 };
        Devices.AddRange(dev1, dev2, dev3, dev4);

        // --- Incidents ---
        var inc1 = new Incident("Unauthorized login attempt detected")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            Priority = Priority.High,
            Status = default,
            Device = dev1
        };
        var inc2 = new Incident("Malware signature found in downloaded file")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            Priority = Priority.Critical,
            Status = default,
            Device = dev2
        };
        var inc3 = new Incident("Unusual outbound traffic spike")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            Priority = Priority.Medium,
            Status = default,
            Device = dev3
        };
        var inc4 = new Incident("Outdated SSL certificate on internal service")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Priority = Priority.Low,
            Status = default,
            Device = dev4
        };
        Incidents.AddRange(inc1, inc2, inc3, inc4);

        // --- Security Measures ---
        SecurityMeasures.AddRange(
            new SecurityMeasure("Reset user credentials and enable MFA", "Credentials reset, MFA active") { Incident = inc1 },
            new SecurityMeasure("Quarantined affected file and ran full AV scan", "File removed, no further threats found") { Incident = inc2 },
            new SecurityMeasure("Blocked suspicious IP range at firewall", "Traffic normalized within 30 minutes") { Incident = inc3 },
            new SecurityMeasure("Renewed SSL certificate and updated configuration", "Certificate valid for 12 months") { Incident = inc4 }
        );

        await SaveChangesAsync();
    }
}
