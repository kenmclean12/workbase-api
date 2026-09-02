using System.ComponentModel.DataAnnotations;
using newproj.DTO.RequestHistory;
using newproj.Models.Base;
using newproj.Models.Users;

namespace newproj.Models.Requests;

public class RequestHistory : EntityBaseClass
{
    public int RequestId { get; set; }

    public Request Request { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

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
            Id = this.Id,
            CreatedAt = this.CreatedAt,
            Request = this.Request.ToResponse(),
            RequestId = this.RequestId,
            UserId = this.UserId,
            User = this.User.ToResponse(),
            Action = this.Action,
        };
    }
}