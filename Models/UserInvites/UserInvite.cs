using System.ComponentModel.DataAnnotations;
using newproj.DTO.User;
using newproj.DTO.UserInvite;
using newproj.Models.Base;
using newproj.Models.Clients;
using newproj.Models.Users;

namespace newproj.Models.UserInvites;

public class UserInvite : EntityBaseClass
{
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = String.Empty;

    public UserRole Role { get; set; } = UserRole.READONLY;
    
    public string TokenHash { get; set; } = String.Empty;

    public int? ClientId { get; set; }

    public Client? Client { get; set; }

    public int InvitedByUserId { get; set; }

    public User InvitedByUser { get; set; } = null!;

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);

    public DateTime? AcceptedAt { get; set; }

    public static UserInvite Create(UserInviteCreateDTO dto)
    {
       var invite = new UserInvite
       {
           Email = dto.Email,
           InvitedByUserId = dto.InvitedByUserId,
           ClientId = dto.ClientId,
       };

       if (dto.Role is UserRole role) invite.Role = role;
       return invite;
    }

    public UserInviteResponseDTO ToResponse()
    {
        var response = new UserInviteResponseDTO
        {
            Id = this.Id,
            CreatedAt = this.CreatedAt,
            Email = this.Email,
            Role = this.Role,
            ClientId = this.ClientId,
            InvitedByUserId = this.InvitedByUserId,
            InvitedByUser = this.InvitedByUser.ToResponse(),
            ExpiresAt = this.ExpiresAt,
        };

        if (this.Client is Client client) response.Client = client.ToResponse();
        return response;
    }
}