using Microsoft.EntityFrameworkCore;
using newproj.Data;

namespace newproj.Services.Requests;

public static class RequestHistoryService
{
    public static void MapRequestHistoryEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/request-history");

        group.MapGet("/request/{requestId:int}", async (AppDbContext db, int requestId) =>
        {
            var historyEntries = await db.RequestHistory
              .Where(rh => rh.RequestId == requestId)
              .Include(rh => rh.User)
              .OrderByDescending(rh => rh.CreatedAt)
              .ToArrayAsync();

            return Results.Ok(
                historyEntries
                  .Select(rh => rh.ToResponse())
                  .ToArray()
            );
        });

        group.MapGet("/user/{userId:int}", async (AppDbContext db, int userId) =>
        {
            var historyEntries = await db.RequestHistory
              .Where(rh => rh.UserId == userId)
              .Include(rh => rh.User)
              .Include(rh => rh.Request)
                .ThenInclude(r => r.Client)
              .OrderByDescending(rh => rh.CreatedAt)
              .ToArrayAsync();

            return Results.Ok(
                historyEntries
                  .Select(rh => rh.ToResponse())
                  .ToArray()
            );
        });
    }
}