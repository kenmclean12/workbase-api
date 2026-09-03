using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.UserInvite;

public class UserInviteBulkCreateDTO
{
    [Required]
    public string[] Emails { get; set; } = [];

    public UserRole? Role { get; set; }

    public int? ClientId { get; set; }

    public int InvitedByUserId { get; set; }
}