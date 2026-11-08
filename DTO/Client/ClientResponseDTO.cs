using newproj.DTO.Address;

namespace newproj.DTO.Client;

public record ClientResponseDTO
{
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string Name { get; init; } = null!;

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Website { get; init; }

    public string? Description { get; init; }

    public AddressResponseDTO? Address { get; init; }
}