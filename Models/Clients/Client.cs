using System.ComponentModel.DataAnnotations;
using newproj.DTO.Client;
using newproj.Models.Addresses;
using newproj.Models.Base;
using newproj.Models.Users;

namespace newproj.Models.Clients;

public class Client : EntityBaseClass
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = String.Empty;

    [MaxLength(50)]
    public string? Email { get; set; }

    [MaxLength(16)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? Website { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public Address? Address { get; set; }

    public ICollection<User> Users { get; set; } = [];

    public static Client Create(ClientCreateDTO dto)
    {
        return new Client
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Website = dto.Website,
            Description = dto.Description,
        };
    }

    public void Update(ClientUpdateDTO dto)
    {
        if (dto.Name is not null) this.Name = dto.Name;
        if (dto.Email is not null) this.Email = dto.Email;
        if (dto.Phone is not null) this.Phone = dto.Phone;
        if (dto.Website is not null) this.Website = dto.Website;
        if (dto.Description is not null) this.Description = dto.Description;
    }

    public ClientResponseDTO ToResponse()
    {
        var response = new ClientResponseDTO
        {
            Name = this.Name,
            Email = this.Email,
            Phone = this.Phone,
            Website = this.Website,
            Description = this.Description,
        };

        if (this.Address is Address address) response.Address = address.ToResponse();
        return response;
    }
}