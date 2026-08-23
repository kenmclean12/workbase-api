using System.ComponentModel.DataAnnotations;
using newproj.DTO.RequestHistory;
using newproj.Models.Base;

namespace newproj.Models.Requests;

public class RequestHistory : EntityBaseClass
{
    public int RequestId { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = null!;

    public static RequestHistory Create(RequestHistoryCreateDTO dto)
    {
        return new RequestHistory
        {
            RequestId = dto.RequestId,
            UserId = dto.UserId,
            Action = dto.Action,
        };
    }

    public RequestHistoryResponseDTO ToResponse()
    {
        return new RequestHistoryResponseDTO
        {
            Id = Id,
            CreatedAt = CreatedAt,
            RequestId = RequestId,
            UserId = UserId,
            Action = Action,
        };
    }
}