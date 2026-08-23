using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.Services.Addresses;
using newproj.Services.Requests;
using newproj.Services.Users;
using newProj.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost", policy =>
    {
        policy.WithOrigins("https://localhost:5173", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddValidation();

var app = builder.Build();
app.UseCors("AllowLocalHost");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.MapUserEndpoints();
app.MapAddressEndpoints();
app.MapClientEndpoints();
app.MapRequestEndpoints();
app.Run();
