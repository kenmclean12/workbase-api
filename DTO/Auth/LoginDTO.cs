using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public record LoginDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; init; } = String.Empty;

    [Required]
    public string Password { get; init; } = String.Empty;
}