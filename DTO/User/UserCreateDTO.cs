using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.User;

public record UserCreateDTO
{
    [Required]
    [MaxLength(25)]
    public string FirstName { get; init; } = null!;

    [Required]
    [MaxLength(25)]
    public string LastName { get; init; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; init; } = null!;

    [Required]
    [MaxLength(50)]
    public string Password { get; init; } = null!;

    public UserRole Role { get; init; } = UserRole.READONLY;

    public int? ClientId { get; init; }
}