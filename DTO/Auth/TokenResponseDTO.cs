namespace newproj.DTO.Auth;

public record TokenResponseDTO(string Token, string RefreshToken, int ExpiresIn = 3000);