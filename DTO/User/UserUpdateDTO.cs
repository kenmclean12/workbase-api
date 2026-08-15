using System.ComponentModel.DataAnnotations;
using newproj.Models.User;

namespace newproj.DTO.User;

public class UserUpdateDTO
{

    [MaxLength(25)]
    public string? FirstName { get; set; } = null;

    [MaxLength(25)]
    public string? LastName { get; set; } = null;

    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; } = null;

    [MaxLength(50)]
    public string? Password { get; set; } = null;

    public UserRole? Role { get; set; } = null;
}