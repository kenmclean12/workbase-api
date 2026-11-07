using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.Services.Auth;
using newproj.Services.Requests;
using newproj.Services.UserInvites;
using newProj.Services.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using newproj.Models.Users;
using System.Security.Claims;
using newproj.Services.Addresses;
using newproj.Services.Users;
using newproj.Services.RequestHistoryRecords;
using newproj.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrManager", policy =>
    {
        policy.RequireRole(UserRole.ADMIN.ToString(), UserRole.SUPERADMIN.ToString());
    });
    options.AddPolicy("BlockReadonly", policy =>
    {
        policy.RequireAssertion(context => !context.User.IsInRole(UserRole.READONLY.ToString()));
    });
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IRequestHistoryService, RequestHistoryService>();
builder.Services.AddValidation();

var app = builder.Build();
app.UseCors("AllowLocalHost");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapAuthEndpoints();
app.MapRequestEndpoints();
app.MapRequestNoteEndpoints();
app.MapUserInviteEndpoints();
app.Run();
