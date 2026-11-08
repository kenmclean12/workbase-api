using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public record RegisterDTO
{
    [Required]
    [MaxLength(25)]
    public string FirstName { get; init; } = String.Empty;

    [Required]
    [MaxLength(25)]
    public string LastName { get; init; } = String.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; init; } = String.Empty;

    [Required]
    public string Password { get; init; } = String.Empty;

    [Required]
    public string InviteToken { get; init; } = String.Empty;
}