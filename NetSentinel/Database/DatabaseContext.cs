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

        // --- Incidents & Security Measures (generated) ---
        var rng = new Random(42);
        var devices = new[] { dev1, dev2, dev3, dev4, dev5 };
        var employees = new[] { emp1, emp2, emp3, emp4, emp5, emp6, emp7 };
        var priorities = (Priority[])Enum.GetValues(typeof(Priority));
        var statuses = (Status[])Enum.GetValues(typeof(Status));

        string[] incidentDescriptions =
        [
            "Unauthorized login attempt detected",
            "Malware signature found in downloaded file",
            "Unusual outbound traffic spike",
            "Outdated SSL certificate on internal service",
            "Failed login attempts from unknown source",
            "Phishing email reported by employee",
            "Suspicious process running on endpoint",
            "Data exfiltration attempt blocked by DLP",
            "Ransomware activity detected on workstation",
            "Privilege escalation attempt in admin portal",
            "Unpatched vulnerability exploited on server",
            "Brute-force attack on VPN gateway",
            "Insider threat: bulk file download detected",
            "DNS poisoning attempt identified",
            "Man-in-the-middle attack on internal network",
            "Zero-day exploit attempt on web application",
            "Unauthorized USB device connected",
            "Account takeover via stolen session token",
            "SQL injection attempt on customer portal",
            "Botnet C2 communication detected",
        ];

        string[] measureDescriptions =
        [
            "Reset user credentials and enforced MFA",
            "Quarantined affected file and ran full AV scan",
            "Blocked suspicious IP range at firewall",
            "Renewed SSL certificate and updated configuration",
            "Isolated affected endpoint from network",
            "Applied emergency patch and rebooted service",
            "Revoked session tokens and alerted user",
            "Deployed WAF rule to block injection pattern",
            "Disabled compromised account and notified manager",
            "Reconfigured DNS resolver and cleared cache",
        ];

        string[] measureResults =
        [
            "Threat neutralized, no further activity observed",
            "File removed, no additional threats found",
            "Traffic normalized within 30 minutes",
            "Certificate valid for 12 months",
            "Endpoint re-imaged and returned to service",
            "Patch applied successfully, vulnerability closed",
            "Session invalidated, user re-authenticated securely",
            "Attack pattern blocked, no data compromised",
            "Account secured, audit log reviewed",
            "DNS resolved correctly after cache flush",
        ];

        // Spread ~200 incidents evenly across the last 12 months
        var now = DateTime.UtcNow;
        var generatedIncidents = new List<Incident>();
        for (int i = 0; i < 200; i++)
        {
            // Pick a random day within the last 365 days
            int daysAgo = rng.Next(0, 365);
            var incident = new Incident(incidentDescriptions[rng.Next(incidentDescriptions.Length)])
            {
                CreatedAt = now.AddDays(-daysAgo),
                Priority = priorities[rng.Next(priorities.Length)],
                Status = statuses[rng.Next(statuses.Length)],
                Device = devices[rng.Next(devices.Length)],
            };
            generatedIncidents.Add(incident);
        }
        Incidents.AddRange(generatedIncidents);

        // Add 1-2 security measures per incident
        foreach (var incident in generatedIncidents)
        {
            int measureCount = rng.Next(1, 3);
            for (int m = 0; m < measureCount; m++)
            {
                SecurityMeasures.Add(new SecurityMeasure(
                    measureDescriptions[rng.Next(measureDescriptions.Length)],
                    measureResults[rng.Next(measureResults.Length)])
                {
                    Incident = incident,
                    PerformedBy = employees[rng.Next(employees.Length)],
                });
            }
        }

        await SaveChangesAsync();
        return true;
    }
}
