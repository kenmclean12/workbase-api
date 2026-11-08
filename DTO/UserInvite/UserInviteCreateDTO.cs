using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.UserInvite;

public record UserInviteCreateDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; init; } = String.Empty;

    public UserRole? Role { get; init; }

    public string TokenHash { get; init; } = null!;

    public int? ClientId { get; init; }

    public int InvitedByUserId { get; init; }
}