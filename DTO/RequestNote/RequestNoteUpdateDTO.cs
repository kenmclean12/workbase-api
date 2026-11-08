using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.RequestNote;

public record RequestNoteUpdateDTO
{
    [Required]
    [MaxLength(2000)]
    public string? Content { get; init; }
}