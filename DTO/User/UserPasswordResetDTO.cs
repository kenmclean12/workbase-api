using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.User;

public record UserPasswordResetDTO
{
    [Required]
    [MaxLength(50)]
    public string ExistingPassword { get; init; } = null!;

    [Required]
    [MaxLength(50)]
    public string NewPassword { get; init; } = null!;
}