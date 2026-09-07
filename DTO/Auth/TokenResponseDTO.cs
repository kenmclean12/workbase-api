namespace newproj.DTO.Auth;

public class TokenResponseDTO
{
    public string Token { get; set; } = String.Empty;

    public string RefreshToken { get; set; } = String.Empty;

    public int ExpiresIn { get; set; } = 3000;
}