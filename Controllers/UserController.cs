using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.User;
using newproj.Services.Users;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> FindAll()
    {
        return Ok(await _userService.FindAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindOne(int id)
    {
        UserResponseDTO? user = await _userService.FindOne(id);
        if (user is null) return NotFound($"No user was found with the provided id: {id}");
        return Ok(user);
    }

    [HttpGet("assignable-users")]
    public async Task<IActionResult> FindAssignableUsers()
    {
        return Ok(await _userService.FindAssignableUsers());
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<IActionResult> FindClientUsers(int clientId)
    {
        return Ok(await _userService.FindClientUsers(clientId));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Create(UserCreateDTO dto)
    {
        UserResponseDTO? user = await _userService.Create(dto);
        if (user is null) return Conflict($"User already exists with the provided email: {dto.Email}");
        return Created($"/users/{user.Id}", user);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Update(UserUpdateDTO dto, int id)
    {
        UserUpdateResult result = await _userService.Update(dto, id);
        return result switch
        {
            UserUpdateResult.UserNotFound => NotFound($"No user was found with the provided id: {id}"),
            UserUpdateResult.DuplicateEmail => Conflict($"User already exists with the provided email: {dto.Email}"),
            UserUpdateResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _userService.Delete(id);
        if (!deleted) return NotFound($"No user was found with the provided id: {id}");
        return NoContent();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(UserPasswordResetDTO dto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id))
        {
            return Forbid("You are not authorized to perform this action");
        }

        ResetPasswordResult result = await _userService.ResetPassword(dto, id);
        return result switch
        {
            ResetPasswordResult.UserNotFound => NotFound($"No user was found with the provided id: {id}"),
            ResetPasswordResult.WrongPassword => BadRequest($"Provided password does not match the password for user id: {id}"),
            ResetPasswordResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
