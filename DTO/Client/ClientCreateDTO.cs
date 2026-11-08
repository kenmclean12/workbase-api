using System.ComponentModel.DataAnnotations;
using newproj.DTO.Address;

namespace newproj.DTO.Client;

public record ClientCreateDTO
{
    [Required]
    [MaxLength(30)]
    public string Name { get; init; } = null!;

    [Required]
    [MaxLength(50)]
    public string InitUserEmail { get; init; } = null!;

    public AddressCreateDTO? AddressDTO { get; init; }

    [MaxLength(50)]
    public string? Email { get; init; }

    [MaxLength(16)]
    public string? Phone { get; init; }

    [MaxLength(50)]
    public string? Website { get; init; }

    [MaxLength(300)]
    public string? Description { get; init; }
}