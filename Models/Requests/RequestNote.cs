using System.ComponentModel.DataAnnotations;
using newproj.DTO.RequestNote;
using newproj.Models.Base;

namespace newproj.Models.Requests;

public class RequestNote : EntityBaseClass
{
    public int RequestId { get; set; }

    public int CreatedByUserId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public static RequestNote Create(RequestNoteCreateDTO dto)
    {
        return new RequestNote
        {
            RequestId = dto.RequestId,
            CreatedByUserId = dto.CreatedByUserId,
            Content = dto.Content,
        };
    }

    public void Update(RequestNoteUpdateDTO dto)
    {
        if (dto.Content is not null)
        {
            Content = dto.Content;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public RequestNoteResponseDTO ToResponse()
    {
        return new RequestNoteResponseDTO
        {
            Id = Id,
            CreatedAt = CreatedAt,
            RequestId = RequestId,
            CreatedByUserId = CreatedByUserId,
            Content = Content,
            UpdatedAt = UpdatedAt,
        };
    }
}