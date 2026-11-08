using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public record RefreshTokenCreateDTO
{
    public int UserId { get; init; }

    [Required]
    public string TokenHash { get; init; } = String.Empty;

    public DateTime ExpiresAt { get; init; }
}