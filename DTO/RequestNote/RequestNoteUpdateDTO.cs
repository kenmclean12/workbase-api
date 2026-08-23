using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.RequestNote;

public class RequestNoteUpdateDTO
{
    [Required]
    [MaxLength(2000)]
    public string? Content { get; set; }
}