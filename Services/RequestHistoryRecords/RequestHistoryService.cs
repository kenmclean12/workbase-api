using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.RequestHistory;
using newproj.Models.Requests;

namespace newproj.Services.RequestHistoryRecords;

public class RequestHistoryService(AppDbContext db) : IRequestHistoryService
{
    private readonly AppDbContext _db = db;

    public async Task<ICollection<RequestHistoryResponseDTO>> GetByRequestId(int requestId)
    {
        ICollection<RequestHistory> history = await _db.RequestHistory
              .Where(rh => rh.RequestId == requestId)
              .Include(rh => rh.User)
              .Include(rh => rh.Request)
                .ThenInclude(r => r.Client)
              .Include(rh => rh.Request)
                .ThenInclude(r => r.CreatedByUser)
              .OrderByDescending(rh => rh.CreatedAt)
              .ToArrayAsync();

        return [.. history.Select(h => h.ToResponse())];
    }

    public async Task<ICollection<RequestHistoryResponseDTO>> GetByUserId(int userId)
    {
      ICollection<RequestHistory> historyEntries = await _db.RequestHistory
              .Where(rh => rh.UserId == userId)
              .Include(rh => rh.User)
              .Include(rh => rh.Request)
                .ThenInclude(r => r.Client)
              .Include(rh => rh.Request)
                .ThenInclude(r => r.CreatedByUser)
              .OrderByDescending(rh => rh.CreatedAt)
              .ToArrayAsync();

      return [.. historyEntries.Select(h => h.ToResponse())];
    }
}