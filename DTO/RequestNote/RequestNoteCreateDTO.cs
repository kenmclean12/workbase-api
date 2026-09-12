using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.RequestNote;

public class RequestNoteCreateDTO
{
    public int RequestId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = null!;
}