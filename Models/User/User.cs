using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using newproj.DTO.User;

namespace newproj.Models.User;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(25)]
    public string FirstName { get; set; } = "";

    [Required]
    [MaxLength(25)]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = "";

    [Required]
    [MaxLength(50)]
    public string PasswordHash { get; set; } = "";

    public UserRole Role { get; set; } = UserRole.READONLY;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private static readonly PasswordHasher<User> _passwordHasher = new();

    public User(UserCreateDTO dto)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Email = dto.Email;
        Role = dto.Role;
        PasswordHash = _passwordHasher.HashPassword(this, dto.Password);
    }

    public void Update(UserUpdateDTO dto)
    {
        //Null Comparisons
        if (dto.FirstName != null) FirstName = dto.FirstName;
        if (dto.LastName != null) LastName = dto.LastName;
        if (dto.Email != null) Email = dto.Email;
        //Pattern Matching w/ Variable Declaration
        if (dto.Role is UserRole role) Role = role;
    }
}
