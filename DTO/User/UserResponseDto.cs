using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using newproj.Models.User;

namespace newproj.DTO.User;

public class UserResponseDto
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

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
}