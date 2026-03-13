using System;
using System.Collections.Generic;
using System.Linq;
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

        var rng = new Random(42);

        // --- Departments (20) ---
        string[] deptNames = [
            "IT", "HR", "Finance", "Security", "Legal",
            "Marketing", "Sales", "Operations", "R&D", "Compliance",
            "Customer Support", "Logistics", "Procurement", "Engineering", "Product",
            "Data Analytics", "DevOps", "QA", "Infrastructure", "Executive",
        ];
        string[] locations = [
            "Building A, Gondor", "Building B, Gondor", "Central Building, Rohan",
            "Secondary Building, Rohan", "North Wing, Mordor", "East Tower, Mordor",
            "West Campus, Shire", "South Block, Shire", "Main Hall, Rivendell",
            "Annex, Rivendell",
        ];
        var generatedDepts = deptNames
            .Select((name, i) => new Department(name, locations[i % locations.Length]))
            .ToList();
        Departments.AddRange(generatedDepts);

        // --- Employees (50) ---
        string[] firstNames = [
            "Aragorn", "Boromir", "Frodo", "Bilbo", "Gimli",
            "Legolas", "Gollum", "Gandalf", "Saruman", "Sauron",
            "Eowyn", "Arwen", "Galadriel", "Elrond", "Treebeard",
            "Pippin", "Merry", "Sam", "Faramir", "Denethor",
            "Theoden", "Eomer", "Grima", "Radagast", "Glorfindel",
            "Celeborn", "Haldir", "Lurtz", "Gothmog", "Shelob",
            "Balrog", "Nazgul", "Ents", "Uruk", "Orc",
            "Rohirrim", "Gondorian", "Hobbit", "Dwarf", "Elf",
            "Ranger", "Wizard", "Steward", "Captain", "Scout",
            "Healer", "Blacksmith", "Merchant", "Scholar", "Guard",
        ];
        var generatedEmployees = firstNames
            .Select((name, i) => new Employee($"EMP{i + 1:D3}", name)
            {
                Department = generatedDepts[rng.Next(generatedDepts.Count)]
            })
            .ToList();
        Employees.AddRange(generatedEmployees);

        // --- Devices (100) ---
        string[] deviceTypes = [
            "MacBook", "Windows PC", "Chromebook", "Workstation", "Tablet",
            "Laptop", "Server", "Raspberry Pi", "Thin Client", "Desktop",
        ];
        var generatedDevices = Enumerable.Range(0, 100).Select(i =>
        {
            int subnet = (i / 254) + 1;
            int host = (i % 254) + 1;
            return new Device
            {
                Name = $"{deviceTypes[i % deviceTypes.Length]} {i + 1}",
                IPAddress = $"192.168.{subnet}.{host}",
                SerialNumber = $"SN-{i + 1:D4}",
                Owner = generatedEmployees[rng.Next(generatedEmployees.Count)],
            };
        }).ToList();
        Devices.AddRange(generatedDevices);

        // --- Incidents & Security Measures (generated) ---
        var devices = generatedDevices;
        var employees = generatedEmployees;
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
                Device = devices[rng.Next(devices.Count)],
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
                    PerformedBy = employees[rng.Next(employees.Count)],
                });
            }
        }

        await SaveChangesAsync();
        return true;
    }
}
