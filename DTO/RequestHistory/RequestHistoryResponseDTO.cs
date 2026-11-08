using newproj.DTO.Request;
using newproj.DTO.User;

namespace newproj.DTO.RequestHistory;

public record RequestHistoryResponseDTO
{
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public int RequestId { get; init; }

    public RequestResponseDTO Request { get; init; } = null!;

    public int UserId { get; init; }

    public UserResponseDTO User { get; init; } = null!;

    public string Action { get; init; } = null!;

    public string? Details { get; init; }
}