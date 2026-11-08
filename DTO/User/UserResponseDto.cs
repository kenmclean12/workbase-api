using newproj.Models.Users;

namespace newproj.DTO.User;

public record UserResponseDTO
{
    public int Id { get; init; }

    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Email { get; init; } = null!;

    public UserRole Role { get; init; }

    public DateTime CreatedAt { get; init; }

    public int? ClientId { get; init; }
}