namespace newproj.DTO.Address;

public record AddressResponseDTO
{
    public int ClientId { get; init; }

    public string Street { get; init; } = null!;

    public string City { get; init; } = null!;

    public string State { get; init; } = null!;

    public string Country { get; init; } = null!;

    public string PostalCode { get; init; } = null!;
}