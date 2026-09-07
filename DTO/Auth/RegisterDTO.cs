using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public class RegisterDTO
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
    public string Password { get; set; } = String.Empty;

    [Required]
    public string InviteToken { get; set; } = String.Empty;
}