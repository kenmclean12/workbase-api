using newproj.Models.Users;

namespace newproj.DTO.User;

public class UserResponseDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ClientId { get; set; }
}