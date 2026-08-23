using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.User;

public class UserPasswordResetDTO
{
    [Required]
    [MaxLength(50)]
    public string ExistingPassword { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string NewPassword { get; set; } = null!;
}