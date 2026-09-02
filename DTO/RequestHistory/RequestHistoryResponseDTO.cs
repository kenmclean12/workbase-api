using newproj.DTO.Request;
using newproj.DTO.User;

namespace newproj.DTO.RequestHistory;

public class RequestHistoryResponseDTO
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RequestId { get; set; }

    public RequestResponseDTO Request { get; set; } = null!;

    public int UserId { get; set; }

    public UserResponseDto User { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? Details { get; set; }
}