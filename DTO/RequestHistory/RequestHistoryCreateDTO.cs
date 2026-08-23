using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.RequestHistory;

public class RequestHistoryCreateDTO
{
    public int RequestId { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = null!;
}

