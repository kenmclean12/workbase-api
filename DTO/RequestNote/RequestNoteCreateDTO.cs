using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.RequestNote;

public record RequestNoteCreateDTO
{
    public int RequestId { get; init; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; init; } = null!;
}