using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public class LoginDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = String.Empty;

    [Required]
    public string Password { get; set; } = String.Empty;
}