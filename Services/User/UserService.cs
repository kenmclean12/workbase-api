using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.User;
using newproj.Models.User;

namespace newproj.Services.Users;

public static class UserService
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/user");

        group.MapGet("/", async (AppDbContext db) =>
        {
            return Results.Ok(
              await db.Users
                .Select(u => u.ToResponse())
                .ToArrayAsync()
            );
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound($"No users found with the provided id: {id}");
            return Results.Ok(user.ToResponse());
        });

        group.MapPost("/", async (AppDbContext db, UserCreateDTO dto) =>
        {
            User? existingEmailUser = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingEmailUser is not null)
            {
                return Results.Conflict($"User already exists with the provided email: {dto.Email}");
            }

            User user = User.Create(dto);
            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created($"/user/{user.Id}", user.ToResponse());
        });

        group.MapPut("/{id:int}", async (AppDbContext db, UserUpdateDTO dto, int id) =>
        {
            User? existingEmailUser = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);
            if (existingEmailUser is not null)
            {
                return Results.Conflict($"User already exists with the provided email: {dto.Email}");
            }

            User? existingUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser is null)
            {
                return Results.NotFound($"No user was found with the provided id: {id}");
            }

            existingUser.Update(dto);
            await db.SaveChangesAsync();

            return Results.Ok(existingUser.ToResponse());
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            User? existingUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser is null)
            {
                return Results.NotFound($"No user was found with the provided id: {id}");
            }

            db.Users.Remove(existingUser);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}