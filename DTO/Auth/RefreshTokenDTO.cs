using System.ComponentModel.DataAnnotations;

namespace newproj.DTO.Auth;

public record RefreshTokenDTO([Required] string RefreshToken);