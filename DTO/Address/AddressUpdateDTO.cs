using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Address;

public class AddressUpdateDTO
{
    [MaxLength(50)]
    public string? Street { get; set; }

    [MaxLength(25)]
    public string? City { get; set; }

    [MaxLength(25)]
    public string? State { get; set; }

    [MaxLength(25)]
    public string? Country { get; set; }

    [MaxLength(25)]
    public string? PostalCode { get; set; }
}