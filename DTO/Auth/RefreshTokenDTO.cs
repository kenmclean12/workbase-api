using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public class RefreshTokenDTO
{
    [Required]
    public string RefreshToken { get; set; } = "";
}