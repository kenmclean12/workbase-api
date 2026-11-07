using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.Services.RequestHistoryRecords;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("request-history")]
public class RequestHistoryController(IRequestHistoryService requestHistoryService) : ControllerBase
{
    public readonly IRequestHistoryService _requestHistoryService = requestHistoryService;

    [HttpGet("request/{requestId:int}")]
    public async Task<IActionResult> FindByRequestId(int requestId)
    {
        return Ok(await _requestHistoryService.FindByRequestId(requestId));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> FindByUserId(int userId)
    {
        return Ok(await _requestHistoryService.FindByUserId(userId));
    }
}