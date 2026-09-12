using newproj.Models.Requests;

namespace newproj.DTO.Request;

public class RequestUpdateDTO
{
    public int? AssignedToUserId { get; set; }

    public RequestStatus? Status { get; set; }
}