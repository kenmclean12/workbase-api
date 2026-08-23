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
    [MaxLength(50)]
    public string PasswordHash { get; set; } = String.Empty;

    public UserRole Role { get; set; } = UserRole.READONLY;

    private static readonly PasswordHasher<User> _passwordHasher = new();

    public static User Create(UserCreateDTO dto)
    {
        User user = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = dto.Role,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        return user;
    }

    public void Update(UserUpdateDTO dto)
    {
        if (dto.FirstName != null) this.FirstName = dto.FirstName;
        if (dto.LastName != null) this.LastName = dto.LastName;
        if (dto.Email != null) this.Email = dto.Email;
        if (dto.Role is UserRole role) this.Role = role;
    }

    public UserResponseDto ToResponse()
    {
        return new UserResponseDto
        {
            Id = this.Id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            Role = this.Role,
            CreatedAt = this.CreatedAt,
        };
    }
}
