using System;
using Microsoft.EntityFrameworkCore;
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

    public DatabaseContext (DbContextOptions<DatabaseContext> options) : base(options) {}
}
