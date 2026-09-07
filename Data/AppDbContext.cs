namespace newproj.Data;
using Microsoft.EntityFrameworkCore;
using newproj.Models.Addresses;
using newproj.Models.Auth;
using newproj.Models.Clients;
using newproj.Models.Requests;
using newproj.Models.UserInvites;
using newproj.Models.Users;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Address> Addresses { get; set; }

    public DbSet<Request> Requests { get; set; }

    public DbSet<RequestNote> RequestNotes { get; set; }

    public DbSet<RequestHistory> RequestHistory { get; set; }

    public DbSet<UserInvite> UserInvites { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
