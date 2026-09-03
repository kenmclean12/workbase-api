using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.UserInvite;

public class UserInviteCreateDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = String.Empty;

    public UserRole? Role { get; set; }

    public int? ClientId { get; set; }

    public int InvitedByUserId { get; set; }
}