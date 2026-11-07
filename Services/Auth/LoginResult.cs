using newproj.DTO.Auth;

namespace newproj.Services.Auth;

public enum LoginFailure
{
    UserNotFound,
    WrongPassword
}

public record LoginResult(LoginFailure? Failure, TokenResponseDTO? Response);