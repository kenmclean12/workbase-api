using System.ComponentModel.DataAnnotations;
using newproj.Models.Requests;

namespace newproj.DTO.Request;

public record RequestCreateDTO
{
    public int ClientId { get; init; }

    [Required]
    [MaxLength(100)]
    public string Title { get; init; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; init; } = null!;

    public RequestPriority Priority { get; init; } = RequestPriority.LOW;
}