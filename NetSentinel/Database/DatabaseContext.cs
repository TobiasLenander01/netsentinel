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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SecurityMeasure>()
            .HasOne(sm => sm.PerformedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }

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
    /// Inserts test data. Returns false (and does nothing) if any data already exists.
    /// Call ClearAllAsync() first to start fresh.
    /// </summary>
    public async Task<bool> SeedAsync()
    {
        bool hasData = await Departments.AnyAsync()
                    || await Employees.AnyAsync()
                    || await Devices.AnyAsync()
                    || await Incidents.AnyAsync()
                    || await SecurityMeasures.AnyAsync();

        if (hasData)
            return false;

        // --- Departments ---
        var deptIT = new Department("IT", "Building A, Gondor");
        var deptHR = new Department("HR", "Bulding B, Gondor");
        var deptFinance = new Department("Finance", "Central Building, Rohan");
        var deptSecurity = new Department("Security", "Secondary Building, Rohan");
        Departments.AddRange(deptIT, deptHR, deptFinance, deptSecurity);

        // --- Employees ---
        var emp1 = new Employee("EMP001", "Aragon")   { Department = deptIT };
        var emp2 = new Employee("EMP002", "Boromir"){ Department = deptIT };
        var emp3 = new Employee("EMP003", "Frodo"){ Department = deptHR };
        var emp4 = new Employee("EMP004", "Bilbo")   { Department = deptFinance };
        var emp5 = new Employee("EMP005", "Gimli"){ Department = deptSecurity };
        var emp6 = new Employee("EMP006", "Legolas")   { Department = deptSecurity };
        var emp7 = new Employee("EMP007", "Gollum")   { Department = deptSecurity };
        Employees.AddRange(emp1, emp2, emp3, emp4, emp5, emp6, emp7);

        // --- Devices ---
        var dev1 = new Device { Name = "Macbook",  IPAddress = "192.168.1.10", SerialNumber = "SN-AA-0001", Owner = emp1 };
        var dev2 = new Device { Name = "Windows PC",   IPAddress = "192.168.1.11", SerialNumber = "SN-BB-0002", Owner = emp2 };
        var dev3 = new Device { Name = "Chromebook",  IPAddress = "192.168.1.20", SerialNumber = "SN-CC-0003", Owner = emp3 };
        var dev4 = new Device { Name = "Workstation", IPAddress = "192.168.1.30", SerialNumber = "SN-DD-0004", Owner = emp4 };
        var dev5 = new Device { Name = "Tablet", IPAddress = "192.168.1.40", SerialNumber = "SN-EE-0005", Owner = emp5 };
        Devices.AddRange(dev1, dev2, dev3, dev4, dev5);

        // --- Incidents ---
        var inc1 = new Incident("Unauthorized login attempt detected")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            Priority = Priority.High,
            Status = Status.Rejected,
            Device = dev1
        };
        var inc2 = new Incident("Malware signature found in downloaded file")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            Priority = Priority.Critical,
            Status = Status.InProgress,
            Device = dev2
        };
        var inc3 = new Incident("Unusual outbound traffic spike")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            Priority = Priority.Medium,
            Status = Status.Completed,
            Device = dev3
        };
        var inc4 = new Incident("Outdated SSL certificate on internal service")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Priority = Priority.Low,
            Status = Status.Approved,
            Device = dev4
        };
        var inc5 = new Incident("Failed login attempts from unknown source")
        {
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Priority = Priority.High,
            Status = Status.Rejected,
            Device = dev5
        };
        Incidents.AddRange(inc1, inc2, inc3, inc4, inc5);

        // --- Security Measures ---
        SecurityMeasures.AddRange(
            new SecurityMeasure("Reset user credentials and enable MFA", "Credentials reset, MFA active") { Incident = inc1, PerformedBy = emp1 },
            new SecurityMeasure("Quarantined affected file and ran full AV scan", "File removed, no further threats found") { Incident = inc2, PerformedBy = emp2 },
            new SecurityMeasure("Blocked suspicious IP range at firewall", "Traffic normalized within 30 minutes") { Incident = inc3, PerformedBy = emp3 },
            new SecurityMeasure("Renewed SSL certificate and updated configuration", "Certificate valid for 12 months") { Incident = inc4, PerformedBy = emp4 }
        );

        await SaveChangesAsync();
        return true;
    }
}
