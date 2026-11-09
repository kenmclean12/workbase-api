using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.RequestNote;
using newproj.Services.RequestNotes;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("request-note")]
public class RequestNoteController(IRequestNoteService requestNoteService) : ControllerBase
{
    private readonly IRequestNoteService _requestNoteService = requestNoteService;

    [HttpGet("request/{requestId:int}")]
    public async Task<IActionResult> FindByRequestId(int requestId)
    {
        return Ok(await _requestNoteService.FindByRequestId(requestId));
    }

    [HttpPost]
    [Authorize(Policy = "BlockReadonly")]
    public async Task<IActionResult> Create(RequestNoteCreateDTO dto)
    {
        if (!int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int createdByUserId))
        {
            return Forbid();
        }

        RequestNoteCreateResult result = await _requestNoteService.Create(dto, createdByUserId);

        return result switch
        {
            RequestNoteCreateResult.RequestNotFound => NotFound($"No request was found with the provided id: {dto.RequestId}"),
            RequestNoteCreateResult.UserNotFound => NotFound($"No user was found with the provided id: {createdByUserId}"),
            RequestNoteCreateResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "BlockReadonly")]
    public async Task<IActionResult> Update(RequestNoteUpdateDTO dto, int id)
    {
        if (!int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Forbid();
        }

        RequestNoteMutationResult result = await _requestNoteService.Update(dto, id, userId);
        return result switch
        {
            RequestNoteMutationResult.NoteNotFound => NotFound($"No request note was found with the provided id: {id}"),
            RequestNoteMutationResult.UnauthorizedUser => Forbid("You are not authorized to edit this request note"),
            RequestNoteMutationResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "BlockReadonly")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Forbid();
        }

        RequestNoteMutationResult result = await _requestNoteService.Delete(id, userId);
        return result switch
        {
            RequestNoteMutationResult.NoteNotFound => NotFound($"No request note was found with the provided id: {id}"),
            RequestNoteMutationResult.UnauthorizedUser => Forbid("You are not authorized to delete this request note"),
            RequestNoteMutationResult.Success => NoContent(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
