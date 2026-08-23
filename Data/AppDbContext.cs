namespace newproj.Data;
using Microsoft.EntityFrameworkCore;
using newproj.Models.Addresses;
using newproj.Models.Clients;
using newproj.Models.Requests;
using newproj.Models.Users;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Address> Addresses { get; set; }

    public DbSet<Request> Requests { get; set; }

}
