using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Request;
using newproj.Helpers.RequestHistoryRecords;
using newproj.Models.Requests;
using newproj.Models.Users;

namespace newproj.Services.Requests;

public static class RequestService
{
    public static void MapRequestEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/request");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var requests = await db.Requests
                .Include(r => r.Client)
                .Include(r => r.CreatedByUser)
                .ToArrayAsync();

            return Results.Ok(
                requests
                    .Select(r => r.ToResponse())
                    .OrderByDescending(r => r.Priority)
                    .OrderByDescending(r => r.UpdatedAt)
                    .ToArray()
            );
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var request = await db.Requests
                .Include(r => r.Client)
                .Include(r => r.CreatedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

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
              .Include(r => r.Client)
              .Include(r => r.CreatedByUser)
              .ToArrayAsync();

            return Results.Ok(
                requests
                  .Select(r => r.ToResponse())
                  .OrderByDescending(r => r.Priority)
                  .OrderByDescending(r => r.UpdatedAt)
                  .ToArray()
            );
        });

        group.MapGet("/user/{userId:int}", async (AppDbContext db, int userId) =>
        {
            var requests = await db.Requests
              .Where(r => r.AssignedToUserId == userId)
              .Include(r => r.Client)
              .Include(r => r.CreatedByUser)
              .ToArrayAsync();

            return Results.Ok(
                requests
                  .Select(r => r.ToResponse())
                  .OrderByDescending(r => r.Priority)
                  .OrderByDescending(r => r.UpdatedAt)
                  .ToArray()
            );
        });

        group.MapPost("/", async (AppDbContext db, RequestCreateDTO dto) =>
        {
            var createdByUser = await db.Users.FindAsync(dto.CreatedByUserId);
            if (createdByUser is null)
            {
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
                if (assignedUser is null)
                {
                    return Results.NotFound($"No user was found to assign this request to with the provided user id: {assignedToUserId}");
                }
            }

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                Request request = Request.Create(dto);
                db.Requests.Add(request);
                await db.SaveChangesAsync();

                RequestHistoryHelpers.CreateRequestHistory(
                    db,
                    request.Id,
                    request.CreatedByUserId,
                    $"Request was created by {createdByUser.FirstName} {createdByUser.LastName}"
                );

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                request.Client = client;
                request.CreatedByUser = createdByUser;
                return Results.Created($"/request/{request.Id}", request.ToResponse());
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        group.MapPatch("/{id:int}", async (AppDbContext db, RequestUpdateDTO dto, int id) =>
        {
            var request = await db.Requests
                .Include(r => r.Client)
                .Include(r => r.CreatedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            if (
                dto.AssignedToUserId is null &&
                dto.Status is null
            )
            {
                return Results.BadRequest($"No values were provided to update request id: {id}");
            }

            User? user = await db.Users.FindAsync(dto.ActionUserId);
            if (user is null)
            {
                return Results.BadRequest($"No user was found with the provided action user id: ${dto.ActionUserId}");
            }

            User? assignedUser = null;
            if (dto.AssignedToUserId is not null) {
                assignedUser = await db.Users.FindAsync(dto.AssignedToUserId);
                request.Status = RequestStatus.IN_PROGRESS;
                if (assignedUser is null)
                {
                    return Results.BadRequest($"No user was found with the provided assigned user id: ${dto.AssignedToUserId}");
                }
            }


            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                var originalRequest = request.ToResponse();
                request.Update(dto);

                RequestHistoryHelpers.CreateRequestUpdateHistory(
                    db,
                    request.Id,
                    user,
                    assignedUser
                );

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Results.Ok(request.ToResponse());
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}