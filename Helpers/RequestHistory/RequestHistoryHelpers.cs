using newproj.Data;
using newproj.DTO.RequestHistory;
using newproj.Models.Requests;
using newproj.Models.Users;

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
        User actionUser,
        User? assignedUser
    )
    {
        string actionText;
        if (assignedUser is not null)
        {
            actionText = $"{actionUser.FirstName} {actionUser.LastName} assigned {assignedUser.FirstName} {assignedUser.LastName} to this request, this client request is in progress.";
        } else
        {
            actionText = $"{actionUser.FirstName} {actionUser.LastName} marked this task complete, this client request has been completed";
        }

        RequestHistoryCreateDTO historyDTO = new()
        {
            RequestId = requestId,
            UserId = actionUser.Id,
            Action = actionText,
        };

        RequestHistory newHistoryEntry = RequestHistory.Create(historyDTO);
        db.RequestHistory.Add(newHistoryEntry);
    }
}

