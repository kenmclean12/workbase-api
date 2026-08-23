using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Client;

public class ClientCreateDTO
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [MaxLength(50)]
    public string? Email { get; set; }

    [MaxLength(16)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? Website { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }
}