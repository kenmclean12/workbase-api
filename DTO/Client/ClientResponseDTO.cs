using newproj.DTO.Address;

namespace newproj.DTO.Client;

public class ClientResponseDTO
{
    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public AddressResponseDTO? Address { get; set; }
}