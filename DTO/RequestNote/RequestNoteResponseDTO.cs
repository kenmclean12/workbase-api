using newproj.DTO.User;

namespace newproj.DTO.RequestNote;

public record RequestNoteResponseDTO
{
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public int RequestId { get; init; }

    public int CreatedByUserId { get; init; }

    public UserResponseDTO CreatedByUser { get; init; } = null!;

    public string Content { get; init; } = null!;

    public DateTime? UpdatedAt { get; init; }
}