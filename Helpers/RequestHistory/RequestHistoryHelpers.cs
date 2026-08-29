using newproj.Data;
using newproj.DTO.Request;
using newproj.DTO.RequestHistory;
using newproj.Models.Requests;

namespace newproj.Helpers.RequestHistoryRecords;

public static class RequestHistoryHelpers
{
    public static void CreateRequestHistory(
        AppDbContext db,
        int requestId,
        int actionUserId,
        string action
    )
    {
        RequestHistoryCreateDTO historyDTO = new()
        {
            RequestId = requestId,
            UserId = actionUserId,
            Action = action,
        };


        RequestHistory newHistoryEntry = RequestHistory.Create(historyDTO);
        db.RequestHistory.Add(newHistoryEntry);
    }

    public static void CreateRequestUpdateHistory(
        AppDbContext db,
        int requestId,
        int actionUserId,
        RequestResponseDTO originalRequest,
        RequestUpdateDTO dto
    )
    {
        List<string> list = [];
        if (dto.Title is not null) list.Add($"Title was changed from {originalRequest.Title} to {dto.Title}");
        if (dto.Description is not null) list.Add($"Description was changed from {originalRequest.Description} to: {dto.Description}");
        if (dto.Status is RequestStatus status) list.Add($"Status was changed from {originalRequest.Status} to {status}");
        if (dto.Priority is RequestPriority priority) list.Add($"Priority was changed from {originalRequest.Priority} to {priority}");

        RequestHistoryCreateDTO historyDTO = new()
        {
            RequestId = requestId,
            UserId = actionUserId,
            Action = $"Request was updated with the following information: {string.Join(", ", list)}",
        };

        RequestHistory newHistoryEntry = RequestHistory.Create(historyDTO);
        db.RequestHistory.Add(newHistoryEntry);
    }
}

