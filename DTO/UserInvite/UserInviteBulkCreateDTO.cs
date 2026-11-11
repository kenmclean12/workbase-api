using System.ComponentModel.DataAnnotations;
using newproj.Models.Users;

namespace newproj.DTO.UserInvite;

public record UserInviteBulkCreateDTO : IValidatableObject
{
    [Required]
    public string[] Emails { get; init; } = [];

    public UserRole? Role { get; init; }

    public int? ClientId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        if (Emails.Any(email => !new EmailAddressAttribute().IsValid(email)))
        {
            yield return new ValidationResult("One or more email addresses are invalid");
        }
    }
}