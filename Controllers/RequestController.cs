using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.Request;
using newproj.Services.Requests;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("request")]
public class RequestController(IRequestService requestService) : ControllerBase
{
    private readonly IRequestService _requestService = requestService;

    [HttpGet]
    public async Task<IActionResult> FindAll()
    {
        return Ok(await _requestService.FindAll());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindOne(int id)
    {
        RequestResponseDTO? request = await _requestService.FindOne(id);
        if (request is null) return NotFound($"No user was found with the provided id {id}");
        return Ok(request);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<IActionResult> FindByClientId(int clientId)
    {
        return Ok(await _requestService.FindByClientId(clientId));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> FindByUserId(int userId)
    {
        return Ok(await _requestService.FindByUserId(userId));
    }

    [HttpPost]
    [Authorize(Policy = "BlockReadonly")]
    public async Task<IActionResult> Create(RequestCreateDTO dto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int createdByUserId))
        {
            return Forbid("No user id provided in token");
        }

        if (!int.TryParse(User.FindFirstValue("client_id"), out int requestUserClientId) || requestUserClientId != dto.ClientId)
        {
            return Forbid("Must be a valid client user to create a request");
        }

        RequestCreateResult result = await _requestService.Create(dto, createdByUserId);
        return result switch
        {
            RequestCreateResult.ClientNotFound => NotFound($"No client was found with the provided id: {dto.ClientId}"),
            RequestCreateResult.UserNotFound => NotFound($"No user was found with the id provided by the token {createdByUserId}"),
            RequestCreateResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "BlockReadonly")]
    public async Task<IActionResult> Update(RequestUpdateDTO dto, int id)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int actionUserId))
        {
            return Forbid("No user id provided in token");
        }

        if (
            dto.AssignedToUserId is null &&
            dto.Status is null
        )
        {
            return BadRequest($"No values were provided to update request id: {id}");
        }

        RequestUpdateResult result = await _requestService.Update(dto, id, actionUserId);
        return result switch
        {
            RequestUpdateResult.RequestNotFound => NotFound($"No request was found with the provided id: {id}"),
            RequestUpdateResult.ActionUserNotFound => NotFound($"No action user was found with the id provided by the token: {actionUserId}"),
            RequestUpdateResult.AssignedUserNotFound => NotFound($"No assigned user was found with the id provided: {dto.AssignedToUserId}"),
            RequestUpdateResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
