using System.ComponentModel.DataAnnotations;
using newproj.DTO.RequestNote;
using newproj.Models.Base;
using newproj.Models.Users;

namespace newproj.Models.Requests;

public class RequestNote : EntityBaseClass
{
    public int RequestId { get; set; }

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

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
            Id = this.Id,
            CreatedAt = this.CreatedAt,
            RequestId = this.RequestId,
            CreatedByUserId = this.CreatedByUserId,
            CreatedByUser = this.CreatedByUser.ToResponse(),
            Content = this.Content,
            UpdatedAt = this.UpdatedAt,
        };
    }
}