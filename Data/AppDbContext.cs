namespace newproj.Data;
using Microsoft.EntityFrameworkCore;
using newproj.Models.User;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}

// public class AppDbContext : DbContext
// {
//     public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
//     public DbSet<User> Users { get; set; }
// }
