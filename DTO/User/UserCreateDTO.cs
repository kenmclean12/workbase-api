using System.ComponentModel.DataAnnotations;
using newproj.Models.User;

namespace newproj.DTO.User;

public class UserCreateDTO
{

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
    public string Password { get; set; } = "";

    public UserRole Role { get; set; } = UserRole.READONLY;
}