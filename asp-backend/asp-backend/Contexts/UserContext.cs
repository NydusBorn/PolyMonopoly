using Microsoft.EntityFrameworkCore;

namespace asp_backend.Contexts;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source=./users.db");
}

public enum UserRole
{
    Guest,
    User,
    Admin
}

[PrimaryKey("Id")]
public class User
{
    public int Id { get; }
    public DateTime Created { get; init; }
    public required UserRole Role { get; set; }
    public required string UserName { get; init; }
    public string? Name { get; set; }
    public string? PasswordHash { get; set; }
}