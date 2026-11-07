using newproj.DTO.Auth;

namespace newproj.Services.Auth;

public enum RefreshAccessTokenFailure
{
    InvalidToken,
    RefreshTokenNotFound,
    UserNotFound
}

public record RefreshAccessTokenResult(RefreshAccessTokenFailure? Failure, TokenResponseDTO? Response);