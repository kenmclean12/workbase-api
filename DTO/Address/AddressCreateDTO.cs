using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Address;

public record AddressCreateDTO
{
    [Required]
    [MaxLength(50)]
    public string Street { get; init; } = null!;

    [Required]
    [MaxLength(25)]
    public string City { get; init; } = null!;

    [Required]
    [MaxLength(25)]
    public string State { get; init; } = null!;

    [Required]
    [MaxLength(25)]
    public string Country { get; init; } = null!;

    [Required]
    [MaxLength(25)]
    public string PostalCode { get; init; } = null!;
}