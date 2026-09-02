using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Address;

public class AddressCreateDTO
{
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(25)]
    public string City { get; set; } = null!;

    [Required]
    [MaxLength(25)]
    public string State { get; set; } = null!;

    [Required]
    [MaxLength(25)]
    public string Country { get; set; } = null!;

    [Required]
    [MaxLength(25)]
    public string PostalCode { get; set; }  = null!;
}