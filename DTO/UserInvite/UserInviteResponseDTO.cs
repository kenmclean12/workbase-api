using newproj.DTO.Client;
using newproj.DTO.User;
using newproj.Models.Users;

namespace newproj.DTO.UserInvite;

public class UserInviteResponseDTO
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Email { get; set; } = String.Empty;

    public UserRole Role { get; set; }

    public int? ClientId { get; set; }

    public ClientResponseDTO? Client { get; set; }

    public int InvitedByUserId { get; set; }

    public UserResponseDTO InvitedByUser { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}