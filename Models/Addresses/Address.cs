using System.ComponentModel.DataAnnotations;
using newproj.DTO.Address;
using newproj.Models.Base;

namespace newproj.Models.Addresses;

public class Address : EntityBaseClass
{
    public int ClientId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = String.Empty;

    [Required]
    [MaxLength(25)]
    public string City { get; set; } = String.Empty;

    [Required]
    [MaxLength(25)]
    public string State { get; set; } = String.Empty;

    [Required]
    [MaxLength(25)]
    public string Country { get; set; } = String.Empty;

    [Required]
    [MaxLength(25)]
    public string PostalCode { get; set; }  = String.Empty;

    public static Address Create(AddressCreateDTO dto)
    {
        return new Address
        {
            ClientId = dto.ClientId,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            PostalCode = dto.PostalCode
        };
    }

    public void Update(AddressUpdateDTO dto)
    {
        if (dto.Street is not null) this.Street = dto.Street;
        if (dto.City is not null) this.City = dto.City;
        if (dto.State is not null) this.State = dto.State;
        if (dto.Country is not null) this.Country = dto.Country;
        if (dto.PostalCode is not null) this.PostalCode = dto.PostalCode;
    }

    public AddressResponseDTO ToResponse()
    {
        return new AddressResponseDTO
        {
            ClientId = this.ClientId,
            Street = this.Street,
            City = this.City,
            State = this.State,
            Country = this.Country,
            PostalCode = this.PostalCode
        };
    }
}