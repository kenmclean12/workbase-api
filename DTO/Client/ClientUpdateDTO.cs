using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Client;

public record ClientUpdateDTO
{
    [MaxLength(30)]
    public string? Name { get; init; }

    [MaxLength(50)]
    public string? Email { get; init; }

    [MaxLength(16)]
    public string? Phone { get; init; }

    [MaxLength(50)]
    public string? Website { get; init; }

    [MaxLength(300)]
    public string? Description { get; init; }
}