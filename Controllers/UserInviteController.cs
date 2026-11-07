using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.UserInvite;
using newproj.Services.UserInvites;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("user-invites")]
public class UserInviteController(IUserInviteService userInviteService) : ControllerBase
{
    private readonly IUserInviteService _userInviteService = userInviteService;

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _userInviteService.FindPending());
    }

    [HttpGet("client/{clientId:int}/pending")]
    public async Task<IActionResult> GetPendingByClientId(int clientId)
    {
        return Ok(await _userInviteService.FindPendingByClientId(clientId));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Create(UserInviteBulkCreateDTO dto)
    {
        if (!int.TryParse(ClaimTypes.NameIdentifier, out int invitedByUserId))
        {
            return Forbid("No user id provided in access token");
        }

        bool created = await _userInviteService.Create(dto, invitedByUserId);
        if (!created) return StatusCode(StatusCodes.Status500InternalServerError);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _userInviteService.Delete(id);
        if (!deleted) return NotFound($"No user invite was found with the provided id: {id}");
        return NoContent();
    }
}