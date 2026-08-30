using newproj.DTO.User;

namespace newproj.DTO.RequestHistory;

public class RequestHistoryResponseDTO
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RequestId { get; set; }

    public int UserId { get; set; }

    public UserResponseDto User { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? Details { get; set; }
}