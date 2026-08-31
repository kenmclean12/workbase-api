using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.RequestNote;
using newproj.Models.Requests;

namespace newproj.Services.Requests;

public static class RequestNoteService
{
    public static void MapRequestNoteEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/request-note");

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

        group.MapPost("/", async (AppDbContext db, RequestNoteCreateDTO dto) =>
        {
            var request = await db.Requests.FindAsync(dto.RequestId);

            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {dto.RequestId}");
            }

            var user = await db.Users.FindAsync(dto.CreatedByUserId);

            if (user is null)
            {
                return Results.NotFound($"No user was found with the provided id: {dto.CreatedByUserId}");
            }

            var note = RequestNote.Create(dto);

            db.RequestNotes.Add(note);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/request-note/{note.Id}",
                note.ToResponse()
            );
        });

        group.MapPut("/{id:int}", async (AppDbContext db, RequestNoteUpdateDTO dto, int id) =>
        {
            var note = await db.RequestNotes.FindAsync(id);

            if (note is null)
            {
                return Results.NotFound($"No request note was found with the provided id: {id}");
            }

            note.Update(dto);
            await db.SaveChangesAsync();

            return Results.Ok(note.ToResponse());
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var note = await db.RequestNotes.FindAsync(id);

            if (note is null)
            {
                return Results.NotFound($"No request note was found with the provided id: {id}");
            }

            db.RequestNotes.Remove(note);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}