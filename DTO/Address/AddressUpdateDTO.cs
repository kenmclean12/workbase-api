using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Address;

public record AddressUpdateDTO
{
    [MaxLength(50)]
    public string? Street { get; init; }

    [MaxLength(25)]
    public string? City { get; init; }

    [MaxLength(25)]
    public string? State { get; init; }

    [MaxLength(25)]
    public string? Country { get; init; }

    [MaxLength(25)]
    public string? PostalCode { get; init; }
}