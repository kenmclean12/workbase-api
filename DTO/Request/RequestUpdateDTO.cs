using System.ComponentModel.DataAnnotations;
using newproj.Models.Requests;

namespace newproj.DTO.Request;

public class RequestUpdateDTO
{
    public int? AssignedToUserId { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public RequestStatus? Status { get; set; }

    public RequestPriority? Priority { get; set; }
}