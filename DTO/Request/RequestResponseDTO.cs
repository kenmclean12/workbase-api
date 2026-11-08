using newproj.Models.Requests;
using newproj.DTO.Client;
using newproj.DTO.User;

namespace newproj.DTO.Request;

public record RequestResponseDTO
{
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public int ClientId { get; init; }

    public ClientResponseDTO Client { get; init; } = null!;

    public int CreatedByUserId { get; init; }

    public UserResponseDTO CreatedByUser { get; init; } = null!;

    public int? AssignedToUserId { get; init; }

    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public RequestStatus Status { get; init; }

    public RequestPriority Priority { get; init; }

    public DateTime? UpdatedAt { get; init; }
}