using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.User;

public record UserUpdateDTO
{
    [MaxLength(25)]
    public string? FirstName { get; init; }

    [MaxLength(25)]
    public string? LastName { get; init; }

    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; init; }

    public UserRole? Role { get; init; }
}