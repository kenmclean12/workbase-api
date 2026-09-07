using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.UserInvite;
using newproj.Models.UserInvites;

namespace newproj.Services.UserInvites;

public static class UserInviteService
{
    public static void MapUserInviteEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/user-invites");

        group.MapGet("/pending", async (AppDbContext db) =>
        {
            var pendingInvites = await db.UserInvites
                .Where(ui => ui.ClientId == null && ui.AcceptedAt == null)
                .Include(ui => ui.InvitedByUser)
                .ToArrayAsync();

            return Results.Ok(pendingInvites.Select(ui => ui.ToResponse()).ToArray());
        });

        group.MapGet("/client/{clientId:int}/pending", async (AppDbContext db, int clientId) =>
        {
            var pendingInvites = await db.UserInvites
                .Where(ui => ui.ClientId == clientId && ui.AcceptedAt == null)
                .Include(ui => ui.Client)
                .Include(ui => ui.InvitedByUser)
                .ToArrayAsync();

            return Results.Ok(pendingInvites.Select(ui => ui.ToResponse()).ToArray());
        });

        group.MapPost("/", async (AppDbContext db, UserInviteBulkCreateDTO dto) =>
        {
            for (int i = 0; i < dto.Emails.Length; i++)
            {
                byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
                string token = Convert.ToBase64String(tokenBytes);
                string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

                var createDTO = new UserInviteCreateDTO
                {
                    Email = dto.Emails[i],
                    Role = dto.Role,
                    ClientId = dto.ClientId,
                    TokenHash = tokenHash,
                    InvitedByUserId = dto.InvitedByUserId,
                };

                var invite = UserInvite.Create(createDTO);
                db.UserInvites.Add(invite);
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var invite = await db.UserInvites.FindAsync(id);
            if (invite is null)
            {
                return Results.NotFound($"No invite was found with the provided id: {id}");
            }

            db.UserInvites.Remove(invite);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}