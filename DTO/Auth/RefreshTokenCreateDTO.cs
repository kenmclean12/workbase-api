using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public class RefreshTokenCreateDTO
{
    public int UserId { get; set; }

    [Required]
    public string TokenHash { get; set; } = String.Empty;

    public DateTime ExpiresAt { get; set; }
}