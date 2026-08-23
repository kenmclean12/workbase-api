using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Request;
using newproj.DTO.RequestHistory;
using newproj.Models.Requests;

namespace newproj.Services.Requests;

public static class RequestService
{
    public static void MapRequestEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/request");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var requests = await db.Requests.ToArrayAsync();
            return Results.Ok(
                requests
                    .Select(r => r.ToResponse())
                    .ToArray()
            );
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var request = await db.Requests.FindAsync(id);
            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            return Results.Ok(request.ToResponse());
        });

        group.MapGet("/client/{clientId:int}", async (AppDbContext db, int clientId) =>
        {
            var requests = await db.Requests
              .Where(r => r.ClientId == clientId)
              .ToArrayAsync();

            return Results.Ok(
                requests
                  .Select(r => r.ToResponse())
                  .ToArray()
            );
        });

        group.MapGet("/user/{userId:int}", async (AppDbContext db, int userId) =>
        {
            var requests = await db.Requests
              .Where(r => r.AssignedToUserId == userId)
              .ToArrayAsync();

            return Results.Ok(
                requests
                  .Select(r => r.ToResponse())
                  .ToArray()
            );
        });

        group.MapPost("/", async (AppDbContext db, RequestCreateDTO dto) =>
        {
            var createdByUser = await db.Users.FindAsync(dto.CreatedByUserId);
            if (createdByUser is null) {
                return Results.NotFound($"No user was found with the provided creating user id: {dto.CreatedByUserId}");
            }

            var client = await db.Clients.FindAsync(dto.ClientId);
            if (client is null)
            {
                return Results.NotFound($"No client was found with the provided client id: {dto.ClientId}");
            }

            if (dto.AssignedToUserId is int assignedToUserId)
            {
                var assignedUser = await db.Users.FindAsync(assignedToUserId);
                if (assignedUser is null) {
                    return Results.NotFound($"No user was found to assign this request to with the provided user id: {assignedToUserId}");
                }
            }

            Request request = Request.Create(dto);
            db.Requests.Add(request);
            await db.SaveChangesAsync();

            RequestHistoryCreateDTO historyDTO = new()
            {
                RequestId = request.Id,
                UserId = request.CreatedByUserId,
                Action = $"Request was created by User = {createdByUser.FirstName} {createdByUser.LastName}, Id={createdByUser.Id}",
            };


            RequestHistory newHistoryEntry = RequestHistory.Create(historyDTO);
            db.RequestHistory.Add(newHistoryEntry);
            await db.SaveChangesAsync();
            
            return Results.Created($"/request/{request.Id}", request.ToResponse());
        });

        group.MapPatch("/{id:int}/unassign", async (AppDbContext db, int id) =>
        {
            var request = await db.Requests.FindAsync(id);
            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            request.Unassign();
            await db.SaveChangesAsync();

            return Results.Ok(request.ToResponse());
        });

        group.MapPatch("/{id:int}", async (AppDbContext db, RequestUpdateDTO dto, int id) =>
        {
            var request = await db.Requests.FindAsync(id);
            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            request.Update(dto);
            await db.SaveChangesAsync();

            return Results.Ok(request.ToResponse());
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var request = await db.Requests.FindAsync(id);
            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            db.Requests.Remove(request);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}