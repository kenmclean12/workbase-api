using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.RequestNote;
using newproj.Models.Requests;
using newproj.Models.Users;

namespace newproj.Services.Requests;

public static class RequestNoteService
{
    public static void MapRequestNoteEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/request-note").RequireAuthorization();

        group.MapGet("/request/{requestId:int}", async (AppDbContext db, int requestId) =>
        {
            var notes = await db.RequestNotes
                .Where(n => n.RequestId == requestId)
                .Include(n => n.CreatedByUser)
                .ToArrayAsync();

            return Results.Ok(
                notes
                    .Select(n => n.ToResponse())
                    .ToArray()
            );
        });

        group.MapPost("/", async (AppDbContext db, HttpContext context, RequestNoteCreateDTO dto) =>
        {
            if (!int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out int createdByUserId))
            {
                return Results.Forbid();
            }

            var request = await db.Requests.FindAsync(dto.RequestId);
            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {dto.RequestId}");
            }

            var user = await db.Users.FindAsync(createdByUserId);

            if (user is null)
            {
                return Results.NotFound($"No user was found with the provided id: {createdByUserId}");
            }

            var note = RequestNote.Create(dto, createdByUserId);

            db.RequestNotes.Add(note);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/request-note/{note.Id}",
                note.ToResponse()
            );
        })
            .RequireAuthorization("BlockReadonly");

        group.MapPut("/{id:int}", async (AppDbContext db, HttpContext context, RequestNoteUpdateDTO dto, int id) =>
        {
            var note = await db.RequestNotes.FindAsync(id);
            if (note is null)
            {
                return Results.NotFound($"No request note was found with the provided id: {id}");
            }

            if (!int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId != note.CreatedByUserId)
            {
                return Results.Forbid();
            }

            note.Update(dto);
            await db.SaveChangesAsync();

            return Results.Ok(note.ToResponse());
        })
            .RequireAuthorization("BlockReadonly");;

        group.MapDelete("/{id:int}", async (AppDbContext db, HttpContext context, int id) =>
        {
            var note = await db.RequestNotes.FindAsync(id);
            if (note is null)
            {
                return Results.NotFound($"No request note was found with the provided id: {id}");
            }

            if (!int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId != note.CreatedByUserId)
            {
                return Results.Forbid();
            }

            db.RequestNotes.Remove(note);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
            .RequireAuthorization("BlockReadonly");
    }
}