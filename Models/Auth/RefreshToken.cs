using System.ComponentModel.DataAnnotations;
using newproj.DTO.Auth;
using newproj.Models.Base;
using newproj.Models.Users;

namespace newproj.Models.Auth;

public class RefreshToken : EntityBaseClass
{
    public int UserId { get; set; }

    [Required]
    public string TokenHash { get; set; } = String.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public static RefreshToken Create(RefreshTokenCreateDTO dto)
    {
        return new RefreshToken()
        {
            UserId = dto.UserId,
            TokenHash = dto.TokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
    }
}