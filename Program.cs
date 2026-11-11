using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.Services.Auth;
using newProj.Services.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using newproj.Models.Users;
using System.Security.Claims;
using newproj.Services.Addresses;
using newproj.Services.Users;
using newproj.Services.RequestHistoryRecords;
using newproj.Services.Clients;
using newproj.Services.RequestNotes;
using newproj.Services.Requests;
using newproj.Services.UserInvites;
using FluentEmail.MailKitSmtp;
using newproj.Services.Emails;

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

builder.Services
    .AddFluentEmail(builder.Configuration["Email:FromEmail"])
    .AddMailKitSender(new SmtpClientOptions
    {
        Server = builder.Configuration["Email:Host"],
        Port = builder.Configuration.GetValue<int>("Email:Port"),
        User = builder.Configuration["Email:Username"],
        Password = builder.Configuration["Email:Password"]
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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "newproj API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste the access token returned by POST /auth/login. The 'Bearer' prefix is added automatically.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            new List<string>()
        }
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddValidation();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserInviteService, UserInviteService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRequestNoteService, RequestNoteService>();
builder.Services.AddScoped<IRequestHistoryService, RequestHistoryService>();

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
app.UseExceptionHandler();
app.MapControllers();

app.Run();
