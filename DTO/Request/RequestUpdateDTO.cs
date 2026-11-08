using newproj.Models.Requests;

namespace newproj.DTO.Request;

public record RequestUpdateDTO
{
    public int? AssignedToUserId { get; init; }

    public RequestStatus? Status { get; init; }
}