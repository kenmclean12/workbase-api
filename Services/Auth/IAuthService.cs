using newproj.DTO.Auth;
using newproj.DTO.User;

namespace newproj.Services.Auth;

public interface IAuthService
{
    Task<LoginResult> Login(LoginDTO dto);

    Task<RegisterResult> Register(RegisterDTO dto);

    Task<RefreshAccessTokenResult> RefreshAccessToken(RefreshTokenDTO dto);

    Task<LogoutResult> Logout(RefreshTokenDTO dto);
}