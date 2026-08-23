using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.User;

public class UserUpdateDTO
{

    [MaxLength(25)]
    public string? FirstName { get; set; }

    [MaxLength(25)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Password { get; set; }

    public UserRole? Role { get; set; }
}