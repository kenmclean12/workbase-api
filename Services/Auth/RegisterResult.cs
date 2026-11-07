using newproj.DTO.Auth;
using newproj.DTO.User;

namespace newproj.Services.Auth;

public enum RegisterFailure
{
    InvalidToken,
    InviteNotFound
}

public record RegisterResult(RegisterFailure? Failure, UserResponseDTO? CreatedUser);