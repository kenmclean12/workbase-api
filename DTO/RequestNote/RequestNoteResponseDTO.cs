namespace newproj.DTO.RequestNote;

public class RequestNoteResponseDTO
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RequestId { get; set; }

    public int CreatedByUserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }
}