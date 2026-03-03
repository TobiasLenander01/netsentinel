using System;
using Microsoft.EntityFrameworkCore;
using NetSentinel.Models;

namespace NetSentinel.Database;

public class DatabaseContext : DbContext
{
    // define database tables
    public DbSet<Device> Devices {get; set;}

    public DatabaseContext (DbContextOptions<DatabaseContext> options) : base(options) {}
}
