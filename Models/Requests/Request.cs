using System.ComponentModel.DataAnnotations;
using newproj.DTO.Request;
using newproj.Models.Base;
using newproj.Models.Clients;
using newproj.Models.Users;

namespace newproj.Models.Requests;

public class Request : EntityBaseClass
{
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public int? AssignedToUserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.UNASSIGNED;

    public RequestPriority Priority { get; set; } = RequestPriority.LOW;

    public bool Internal { get; set; } = false;

    public DateTime? UpdatedAt { get; set; }

    public static Request Create(RequestCreateDTO dto)
    {
        return new Request
        {
            ClientId = dto.ClientId,
            CreatedByUserId = dto.CreatedByUserId,
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Internal = dto.Internal,
        };
    }

    public void Update(RequestUpdateDTO dto)
    {
        if (dto.AssignedToUserId is int assignedToUserId) this.AssignedToUserId = assignedToUserId;
        if (dto.Status is RequestStatus status) this.Status = status;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public RequestResponseDTO ToResponse()
    {
        return new RequestResponseDTO
        {
            Id = this.Id,
            CreatedAt = this.CreatedAt,
            ClientId = this.ClientId,
            Client = this.Client.ToResponse(),
            CreatedByUserId = this.CreatedByUserId,
            CreatedByUser = this.CreatedByUser.ToResponse(),
            AssignedToUserId = this.AssignedToUserId,
            Title = this.Title,
            Description = this.Description,
            Status = this.Status,
            Priority = this.Priority,
            Internal = this.Internal,
            UpdatedAt = this.UpdatedAt,
        };
    }
}