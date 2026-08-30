using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Request;
using newproj.Helpers.RequestHistoryRecords;
using newproj.Models.Requests;

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
                    $"Request was created by User Id={createdByUser.Id}"
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

        group.MapPatch("/{id:int}/unassign", async (AppDbContext db, int id, int actionUserId) =>
        {
            var request = await db.Requests
                .Include(r => r.Client)
                .Include(r => r.CreatedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            if (request.AssignedToUserId is not int assignedUserId)
            {
                return Results.NotFound($"No user was assigned to this request, request id: {id}");
            }

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                request.Unassign();

                RequestHistoryHelpers.CreateRequestHistory(
                    db,
                    request.Id,
                    actionUserId,
                    $"User Id={assignedUserId} was unassigned from this Request, by User Id={actionUserId}"
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

        group.MapPatch("/{id:int}", async (AppDbContext db, RequestUpdateDTO dto, int id, int actionUserId) =>
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
                dto.Title is null &&
                dto.Description is null &&
                dto.Status is null &&
                dto.Priority is null
            )
            {
                return Results.BadRequest($"No values were provided to update request id: {id}");
            }

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                var originalRequest = request.ToResponse();
                request.Update(dto);

                RequestHistoryHelpers.CreateRequestUpdateHistory(
                    db,
                    request.Id,
                    actionUserId,
                    originalRequest,
                    dto
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

        group.MapDelete("/{id:int}", async (AppDbContext db, int id, int actionUserId) =>
        {
            var request = await db.Requests.FindAsync(id);
            if (request is null)
            {
                return Results.NotFound($"No request was found with the provided id: {id}");
            }

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                db.Requests.Remove(request);

                RequestHistoryHelpers.CreateRequestHistory(
                    db,
                    id,
                    actionUserId,
                    $"User Id={actionUserId} deleted this request"
                );

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Results.NoContent();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}