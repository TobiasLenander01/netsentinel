using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NetSentinel.Database;

public class DatabaseMigration : IDesignTimeDbContextFactory<DatabaseContext>
{
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
