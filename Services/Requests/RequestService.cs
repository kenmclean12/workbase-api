using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Request;
using newproj.Helpers.RequestHistoryRecords;
using newproj.Models.Clients;
using newproj.Models.Requests;
using newproj.Models.Users;

namespace newproj.Services.Requests;

public class RequestService(AppDbContext db) : IRequestService
{
    private readonly AppDbContext _db = db;

    public async Task<ICollection<RequestResponseDTO>> FindAll()
    {
        ICollection<Request> requests = await _db.Requests
                .Include(r => r.Client)
                .Include(r => r.CreatedByUser)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .ToArrayAsync();


        return [.. requests.Select(r => r.ToResponse())];
    }

    public async Task<RequestResponseDTO?> FindOne(int id)
    {
        Request? request = await _db.Requests
                .Include(r => r.Client)
                .Include(r => r.CreatedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null) return null;

        return request.ToResponse();
    }

    public async Task<ICollection<RequestResponseDTO>> FindByClientId(int clientId)
    {
        ICollection<Request> requests = await _db.Requests
              .Where(r => r.ClientId == clientId)
              .Include(r => r.Client)
              .Include(r => r.CreatedByUser)
              .OrderByDescending(r => r.UpdatedAt)
                  .ThenByDescending(r => r.CreatedAt)
              .ToArrayAsync();

        return [.. requests.Select(r => r.ToResponse())];
    }

    public async Task<ICollection<RequestResponseDTO>> FindByUserId(int userId)
    {
        ICollection<Request> requests = await _db.Requests
              .Where(r => r.AssignedToUserId == userId)
              .Include(r => r.Client)
              .Include(r => r.CreatedByUser)
              .OrderByDescending(r => r.UpdatedAt)
              .ThenByDescending(r => r.CreatedAt)
              .ToArrayAsync();

        return [.. requests.Select(r => r.ToResponse())];
    }

    public async Task<RequestCreateResult> Create(RequestCreateDTO dto, int createdByUserId)
    {
        User? createdByUser = await _db.Users.FindAsync(createdByUserId);
        if (createdByUser is null) return RequestCreateResult.UserNotFound;

        Client? client = await _db.Clients.FindAsync(dto.ClientId);
        if (client is null) return RequestCreateResult.ClientNotFound;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            Request request = Request.Create(dto, createdByUserId);
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();

            RequestHistoryHelpers.CreateRequestHistory(
                _db,
                request.Id,
                request.CreatedByUserId,
                $"Request was created by {createdByUser.FirstName} {createdByUser.LastName}"
            );

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            request.Client = client;
            request.CreatedByUser = createdByUser;
            return RequestCreateResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<RequestUpdateResult> Update(RequestUpdateDTO dto, int id, int actionUserId)
    {
        Request? request = await _db.Requests
            .Include(r => r.Client)
            .Include(r => r.CreatedByUser)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null) return RequestUpdateResult.RequestNotFound;

        User? actionUser = await _db.Users.FindAsync(actionUserId);
        if (actionUser is null) return RequestUpdateResult.ActionUserNotFound;

        User? assignedUser = null;
        if (dto.AssignedToUserId is not null)
        {
            assignedUser = await _db.Users.FindAsync(dto.AssignedToUserId);
            if (assignedUser is null)
            {
                return RequestUpdateResult.AssignedUserNotFound;
            }
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            request.Update(dto);

            RequestHistoryHelpers.CreateRequestUpdateHistory(
                _db,
                request.Id,
                actionUser,
                assignedUser,
                dto.Status
            );

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return RequestUpdateResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}