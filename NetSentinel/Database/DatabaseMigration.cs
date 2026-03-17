using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

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

public class DatabaseMigration : IDesignTimeDbContextFactory<DatabaseContext>
{
    // This method is called by the EF Core tools at design time to create an instance of the DbContext.
    // Only used for running migrations, not at runtime in the app.
    public DatabaseContext CreateDbContext(string[] args)
    {
        // 1. Build configuration to read the JSON file
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // 2. Get the connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 3. Configure DbContext
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseAzureSql(connectionString);

        return new DatabaseContext(optionsBuilder.Options);
    }
}
