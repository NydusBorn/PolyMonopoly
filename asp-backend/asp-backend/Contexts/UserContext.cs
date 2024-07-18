using Microsoft.EntityFrameworkCore;

namespace asp_backend.Contexts;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=./users.db");
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}