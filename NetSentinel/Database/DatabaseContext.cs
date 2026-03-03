using System;
using Microsoft.EntityFrameworkCore;

namespace NetSentinel.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext (DbContextOptions<DatabaseContext> options) : base(options) {}
}
