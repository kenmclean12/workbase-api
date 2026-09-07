using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using newproj.DTO.User;
using newproj.Models.Base;

namespace newproj.Models.Users;

public class User : EntityBaseClass
{

    [Required]
    [MaxLength(25)]
    public string FirstName { get; set; } = String.Empty;

    [Required]
    [MaxLength(25)]
    public string LastName { get; set; } = String.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = String.Empty;

    [Required]
    public string PasswordHash { get; set; } = String.Empty;

    public UserRole Role { get; set; } = UserRole.READONLY;

    public int? ClientId { get; set; }

    private static readonly PasswordHasher<User> _passwordHasher = new();

    public static User Create(UserCreateDTO dto)
    {
        User user = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            ClientId = dto.ClientId,
            Role = dto.Role,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        if (dto.ClientId is int clientId) user.ClientId = clientId;
        return user;
    }

    public void Update(UserUpdateDTO dto)
    {
        if (dto.FirstName != null) this.FirstName = dto.FirstName;
        if (dto.LastName != null) this.LastName = dto.LastName;
        if (dto.Email != null) this.Email = dto.Email;
        if (dto.Role is UserRole role) this.Role = role;
    }

    public bool VerifyPassword(string password)
    {
        return _passwordHasher.VerifyHashedPassword(this, this.PasswordHash, password) != PasswordVerificationResult.Failed;
    }

    public bool ResetPassword(UserPasswordResetDTO dto)
    {
        var result = _passwordHasher.VerifyHashedPassword(this, this.PasswordHash, dto.ExistingPassword);
        if (result != PasswordVerificationResult.Failed)
        {
            var newPasswordHash = _passwordHasher.HashPassword(this, dto.NewPassword);
            this.PasswordHash = newPasswordHash;
            return true;
        }

        return false;
    }

    public UserResponseDTO ToResponse()
    {
        return new UserResponseDTO
        {
            Id = this.Id,
            CreatedAt = this.CreatedAt,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            Role = this.Role,
            ClientId = this.ClientId,
        };
    }
}
