using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.UserInvite;

public record UserInviteBulkCreateDTO
{
    [Required]
    public string[] Emails { get; init; } = [];

    public UserRole? Role { get; init; }

    public int? ClientId { get; init; }
}