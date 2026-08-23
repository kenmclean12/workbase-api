using System.ComponentModel.DataAnnotations;
using newproj.Models.Requests;

namespace newproj.DTO.Request;

public class RequestCreateDTO
{
    public int ClientId { get; set; }

    public int CreatedByUserId { get; set; }

    public int? AssignedToUserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.UNASSIGNED;

    public RequestPriority Priority { get; set; } = RequestPriority.LOW;

    public bool Internal { get; set; } = false;
}