using System.ComponentModel.DataAnnotations;
using newproj.DTO.Address;

namespace newproj.DTO.Client;

public class ClientCreateDTO
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string InitUserEmail { get; set; } = null!;

    public AddressCreateDTO? AddressDTO { get; set; }

    [MaxLength(50)]
    public string? Email { get; set; }

    [MaxLength(16)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? Website { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }
}