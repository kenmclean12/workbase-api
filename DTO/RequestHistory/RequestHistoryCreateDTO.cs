using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.RequestHistory;

public record RequestHistoryCreateDTO
{
    public int RequestId { get; init; }

    public int UserId { get; init; }

    [Required]
    [MaxLength(100)]
    public string Action { get; init; } = null!;
}