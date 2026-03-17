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
        // Delete behavior summary: Departments/Devices are protected by Restrict, while Employee deletions null out Device.Owner and SecurityMeasure.PerformedBy.
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Device>()
            .HasOne(d => d.Owner)
            .WithMany(e => e.Devices)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Device)
            .WithMany(d => d.Incidents)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SecurityMeasure>()
            .HasOne(sm => sm.PerformedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
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
    /// Returns true if any table contains data. Used to prevent seeding test data on top of real data.
    /// </summary>
    public async Task<bool> HasAnyDataAsync()
    {
        return await Departments.AnyAsync()
            || await Employees.AnyAsync()
            || await Devices.AnyAsync()
            || await Incidents.AnyAsync()
            || await SecurityMeasures.AnyAsync();
    }

    /// <summary>
    /// Inserts test data. Returns false (and does nothing) if any data already exists.
    /// </summary>
    public async Task<bool> SeedAsync()
    {
        if (await HasAnyDataAsync())
            return false;

        var now = DateTime.UtcNow;

        // --- Departments (6) ---
        var deptIt = new Department("IT", "Building A, Gondor");
        var deptHr = new Department("HR", "Building B, Gondor");
        var deptFinance = new Department("Finance", "Central Building, Rohan");
        var deptSecurity = new Department("Security", "North Wing, Mordor");
        var deptOps = new Department("Operations", "West Campus, Shire");
        var deptRnd = new Department("R&D", "Main Hall, Rivendell");
        Departments.AddRange(deptIt, deptHr, deptFinance, deptSecurity, deptOps, deptRnd);

        // --- Employees (12) ---
        var emp1 = new Employee("EMP001", "Aragorn Elessar", "aragorn.elessar@netsentinel.se", deptSecurity);
        var emp2 = new Employee("EMP002", "Boromir Denethor", "boromir.denethor@netsentinel.se", deptOps);
        var emp3 = new Employee("EMP003", "Frodo Baggins", "frodo.baggins@netsentinel.se", deptIt);
        var emp4 = new Employee("EMP004", "Bilbo Baggins", "bilbo.baggins@netsentinel.se", deptFinance);
        var emp5 = new Employee("EMP005", "Gimli son of Gloin", "gimli.gloin@netsentinel.se", deptOps);
        var emp6 = new Employee("EMP006", "Legolas Greenleaf", "legolas.greenleaf@netsentinel.se", deptSecurity);
        var emp7 = new Employee("EMP007", "Gandalf Mithrandir", "gandalf.mithrandir@netsentinel.se", deptRnd);
        var emp8 = new Employee("EMP008", "Eowyn Dernhelm", "eowyn.dernhelm@netsentinel.se", deptHr);
        var emp9 = new Employee("EMP009", "Arwen Undomiel", "arwen.undomiel@netsentinel.se", deptIt);
        var emp10 = new Employee("EMP010", "Galadriel Artanis", "galadriel.artanis@netsentinel.se", deptRnd);
        var emp11 = new Employee("EMP011", "Elrond Halfelven", "elrond.halfelven@netsentinel.se", deptFinance);
        var emp12 = new Employee("EMP012", "Faramir Ithilien", "faramir.ithilien@netsentinel.se", deptSecurity);
        Employees.AddRange(emp1, emp2, emp3, emp4, emp5, emp6, emp7, emp8, emp9, emp10, emp11, emp12);

        // --- Devices (15) ---
        var device1 = new Device("MacBook 1", "192.168.1.10", "SN-0001", emp3);
        var device2 = new Device("Windows PC 2", "192.168.1.11", "SN-0002", emp4);
        var device3 = new Device("Chromebook 3", "192.168.1.12", "SN-0003", emp8);
        var device4 = new Device("Workstation 4", "192.168.1.13", "SN-0004", emp1);
        var device5 = new Device("Tablet 5", "192.168.1.14", "SN-0005", emp2);
        var device6 = new Device("Laptop 6", "192.168.1.15", "SN-0006", emp9);
        var device7 = new Device("Server 7", "192.168.1.16", "SN-0007", emp7);
        var device8 = new Device("Raspberry Pi 8", "192.168.1.17", "SN-0008", emp10);
        var device9 = new Device("Thin Client 9", "192.168.1.18", "SN-0009", emp5);
        var device10 = new Device("Desktop 10", "192.168.1.19", "SN-0010", emp6);
        var device11 = new Device("MacBook 11", "192.168.1.20", "SN-0011", emp11);
        var device12 = new Device("Windows PC 12", "192.168.1.21", "SN-0012", emp12);
        var device13 = new Device("Chromebook 13", "192.168.1.22", "SN-0013", emp3);
        var device14 = new Device("Workstation 14", "192.168.1.23", "SN-0014", emp2);
        var device15 = new Device("Tablet 15", "192.168.1.24", "SN-0015", emp8);
        Devices.AddRange(device1, device2, device3, device4, device5, device6, device7, device8, device9, device10, device11, device12, device13, device14, device15);

        // --- Incidents (18) ---
        var incident1 = new Incident("Unauthorized login attempt detected", now.AddDays(-4), Priority.High, Status.InProgress, device1);
        var incident2 = new Incident("Malware signature found in downloaded file", now.AddDays(-7), Priority.Medium, Status.Approved, device2);
        var incident3 = new Incident("Unusual outbound traffic spike", now.AddDays(-10), Priority.High, Status.InProgress, device3);
        var incident4 = new Incident("Outdated SSL certificate on internal service", now.AddDays(-14), Priority.Low, Status.Completed, device7);
        var incident5 = new Incident("Failed login attempts from unknown source", now.AddDays(-18), Priority.Medium, Status.Rejected, device4);
        var incident6 = new Incident("Phishing email reported by employee", now.AddDays(-23), Priority.Medium, Status.Approved, device5);
        var incident7 = new Incident("Suspicious process running on endpoint", now.AddDays(-31), Priority.High, Status.InProgress, device6);
        var incident8 = new Incident("Data exfiltration attempt blocked by DLP", now.AddDays(-37), Priority.Critical, Status.InProgress, device9);
        var incident9 = new Incident("Ransomware activity detected on workstation", now.AddDays(-45), Priority.Critical, Status.Completed, device10);
        var incident10 = new Incident("Privilege escalation attempt in admin portal", now.AddDays(-56), Priority.High, Status.InProgress, device11);
        var incident11 = new Incident("Unpatched vulnerability exploited on server", now.AddDays(-68), Priority.Critical, Status.Approved, device7);
        var incident12 = new Incident("Brute-force attack on VPN gateway", now.AddDays(-79), Priority.High, Status.Completed, device12);
        var incident13 = new Incident("Insider threat: bulk file download detected", now.AddDays(-92), Priority.High, Status.InProgress, device13);
        var incident14 = new Incident("DNS poisoning attempt identified", now.AddDays(-110), Priority.Medium, Status.Completed, device14);
        var incident15 = new Incident("Man-in-the-middle attempt on internal network", now.AddDays(-130), Priority.High, Status.InProgress, device15);
        var incident16 = new Incident("Zero-day exploit attempt on web application", now.AddDays(-155), Priority.Critical, Status.Approved, device8);
        var incident17 = new Incident("Unauthorized USB device connected", now.AddDays(-180), Priority.Medium, Status.Completed, device5);
        var incident18 = new Incident("SQL injection attempt on customer portal", now.AddDays(-210), Priority.High, Status.InProgress, device2);
        Incidents.AddRange(incident1, incident2, incident3, incident4, incident5, incident6, incident7, incident8, incident9, incident10, incident11, incident12, incident13, incident14, incident15, incident16, incident17, incident18);

        // --- Security Measures (23) ---
        var measure1 = new SecurityMeasure("Reset user credentials and enforced MFA", "Threat neutralized, no further activity observed", incident1, emp1, incident1.CreatedAt.AddHours(3));
        var measure2 = new SecurityMeasure("Quarantined affected file and ran full AV scan", "File removed, no additional threats found", incident2, emp4, incident2.CreatedAt.AddHours(5));
        var measure3 = new SecurityMeasure("Blocked suspicious IP range at firewall", "Traffic normalized within 30 minutes", incident3, emp6, incident3.CreatedAt.AddHours(4));
        var measure4 = new SecurityMeasure("Renewed SSL certificate and updated configuration", "Certificate valid for 12 months", incident4, emp7, incident4.CreatedAt.AddHours(2));
        var measure5 = new SecurityMeasure("Isolated affected endpoint from network", "Endpoint re-imaged and returned to service", incident7, emp9, incident7.CreatedAt.AddHours(6));
        var measure6 = new SecurityMeasure("Applied emergency patch and rebooted service", "Patch applied successfully, vulnerability closed", incident11, emp10, incident11.CreatedAt.AddHours(7));
        var measure7 = new SecurityMeasure("Revoked session tokens and alerted user", "Session invalidated, user re-authenticated securely", incident10, emp12, incident10.CreatedAt.AddHours(3));
        var measure8 = new SecurityMeasure("Deployed WAF rule to block injection pattern", "Attack pattern blocked, no data compromised", incident18, emp3, incident18.CreatedAt.AddHours(4));
        var measure9 = new SecurityMeasure("Disabled compromised account and notified manager", "Account secured, audit log reviewed", incident5, emp2, incident5.CreatedAt.AddHours(5));
        var measure10 = new SecurityMeasure("Reconfigured DNS resolver and cleared cache", "DNS resolved correctly after cache flush", incident14, emp11, incident14.CreatedAt.AddHours(2));
        var measure11 = new SecurityMeasure("Revoked OAuth tokens and rotated secrets", "Secrets rotated, access validated", incident15, emp8, incident15.CreatedAt.AddHours(3));
        var measure12 = new SecurityMeasure("Disabled vulnerable service and opened change ticket", "Service isolated pending remediation", incident12, emp7, incident12.CreatedAt.AddHours(4));
        var measure13 = new SecurityMeasure("Forced password reset and sent advisory", "Passwords reset, users confirmed", incident6, emp1, incident6.CreatedAt.AddHours(2));
        var measure14 = new SecurityMeasure("Updated IDS signatures and validated alerts", "Alerts verified, no false positives", incident3, emp10, incident3.CreatedAt.AddHours(6));
        var measure15 = new SecurityMeasure("Restricted admin portal to trusted subnet", "Access restricted, admin portal stable", incident10, emp6, incident10.CreatedAt.AddHours(8));
        var measure16 = new SecurityMeasure("Implemented rate limiting on login endpoint", "Login spikes reduced and monitored", incident1, emp4, incident1.CreatedAt.AddHours(9));
        var measure17 = new SecurityMeasure("Blocked malicious domain at DNS sinkhole", "Malicious domain sinkholed successfully", incident9, emp12, incident9.CreatedAt.AddHours(3));
        var measure18 = new SecurityMeasure("Migrated affected server to clean image", "Server rebuilt from clean baseline", incident11, emp7, incident11.CreatedAt.AddHours(10));
        var measure19 = new SecurityMeasure("Enabled audit logging with SIEM forwarding", "Logs flowing to SIEM with alerts", incident8, emp5, incident8.CreatedAt.AddHours(6));
        var measure20 = new SecurityMeasure("Removed unauthorized USB device and locked port", "USB policy enforced, no repeat", incident17, emp9, incident17.CreatedAt.AddHours(1));
        var measure21 = new SecurityMeasure("Reset VPN keys and reissued certificates", "VPN certificates renewed and tested", incident12, emp2, incident12.CreatedAt.AddHours(5));
        var measure22 = new SecurityMeasure("Segmented network and enforced ACLs", "ACLs enforced, lateral movement blocked", incident8, emp6, incident8.CreatedAt.AddHours(9));
        var measure23 = new SecurityMeasure("Applied database input validation hotfix", "SQL injection blocked and tests passed", incident18, emp3, incident18.CreatedAt.AddHours(7));
        SecurityMeasures.AddRange(measure1, measure2, measure3, measure4, measure5, measure6, measure7, measure8, measure9, measure10, measure11, measure12, measure13, measure14, measure15, measure16, measure17, measure18, measure19, measure20, measure21, measure22, measure23);

        await SaveChangesAsync();
        return true;
    }
}
