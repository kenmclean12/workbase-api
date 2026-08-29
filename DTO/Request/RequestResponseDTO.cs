using newproj.Models.Requests;
using newproj.DTO.Client;

namespace newproj.DTO.Request;

public class RequestResponseDTO
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int ClientId { get; set; }

    public ClientResponseDTO Client { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public int? AssignedToUserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public RequestStatus Status { get; set; }

    public RequestPriority Priority { get; set; }

    public bool Internal { get; set; }

    public DateTime? UpdatedAt { get; set; }
}