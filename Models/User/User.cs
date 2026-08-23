using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using newproj.DTO.User;
using Npgsql.Replication;

namespace newproj.Models.User;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(25)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(25)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.READONLY;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

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
        if (dto.FirstName != null) FirstName = dto.FirstName;
        if (dto.LastName != null) LastName = dto.LastName;
        if (dto.Email != null) Email = dto.Email;
        if (dto.Role is UserRole role) Role = role;
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
