using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.Auth;
using newproj.Services.Auth;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        LoginResult result = await _authService.Login(dto);
        if (result.Failure is not null)
        {
            return result.Failure switch
            {
                LoginFailure.UserNotFound => NotFound($"No user was found with the provided email: {dto.Email}"),
                LoginFailure.WrongPassword => BadRequest($"Password does not match the stored password for user email: {dto.Email}"),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        return Ok(result.Response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        RegisterResult result = await _authService.Register(dto);
        if (result.Failure is not null)
        {
            return result.Failure switch
            {
                RegisterFailure.InvalidToken => Forbid("Failed to convert invite token to byte array"),
                RegisterFailure.InviteNotFound => NotFound($"No invite was found for the provided email: {dto.Email}"),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        return Ok(result.CreatedUser);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDTO dto)
    {
        RefreshAccessTokenResult result = await _authService.RefreshAccessToken(dto);
        if (result.Failure is not null)
        {
            return result.Failure switch
            {
                RefreshAccessTokenFailure.InvalidToken => Forbid("Failed to convert refresh token to byte array"),
                RefreshAccessTokenFailure.RefreshTokenNotFound => NotFound("Refresh token not found with the provided token string"),
                RefreshAccessTokenFailure.UserNotFound => NotFound($"No user was found with the id provided by the matched refresh token"),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        return Ok(result.Response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDTO dto)
    {
        LogoutResult result = await _authService.Logout(dto);
        return result switch
        {
            LogoutResult.InvalidToken => Forbid("Failed to convert refresh token to byte array"),
            LogoutResult.RefreshTokenNotFound => NotFound("No refresh token entry was found matching the provided token string"),
            LogoutResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}